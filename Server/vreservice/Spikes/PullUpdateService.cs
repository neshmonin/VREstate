﻿using System;
using System.Collections.Generic;
using Vre.Server.BusinessLogic;

namespace Vre.Server.Spikes
{
    internal class PullUpdateService
    {
        public enum EntityLevel : int { Developer = 0, Site = 1, Building = 2, Suite = 3 }

        private class GenerationEntity
        {
            public GenerationEntity(EntityLevel level, int id, GenerationEntity parent, long updateTime)
            {
                Level = level;
                Id = id;
                Parent = parent;
                LastUpdateTime = updateTime;
                ListNode = null;
            }
            public EntityLevel Level;
            public int Id;
            public GenerationEntity Parent;
            public long LastUpdateTime;
            public LinkedListNode<GenerationEntity> ListNode;
        }

        public class UpdateInfo
        {
            public List<int> Developers = null, Sites = null, Buildings = null, Suites = null;
            public long Generation;
        }

        private Dictionary<int, GenerationEntity> _developerXRef = new Dictionary<int,GenerationEntity>();
        private Dictionary<int, GenerationEntity> _siteXRef = new Dictionary<int,GenerationEntity>();
        private Dictionary<int, GenerationEntity> _buildingXRef = new Dictionary<int,GenerationEntity>();
        private Dictionary<int, GenerationEntity> _suiteXRef = new Dictionary<int,GenerationEntity>();
        private LinkedList<GenerationEntity> _updateList = new LinkedList<GenerationEntity>();

        /// <summary>
        /// Update result set with generation information.
        /// </summary>
        /// <param name="level">root entity level</param>
        /// <param name="entityId">root entity id</param>
        /// <param name="generationQueryValue">current generation value known by client;
        /// if this is zero, result set is updated with current generation value only.</param>
        public UpdateInfo GetUpdate(EntityLevel level, int entityId, long generationQueryValue)
        {
            UpdateInfo result = new UpdateInfo();
            result.Generation = 0;

            GenerationEntity topEntity = null;
            switch (level)
            {
                case EntityLevel.Developer:
                    lock (_developerXRef) _developerXRef.TryGetValue(entityId, out topEntity);
                    break;

                case EntityLevel.Site:
                    lock (_siteXRef) _siteXRef.TryGetValue(entityId, out topEntity);
                    break;

                case EntityLevel.Building:
                    lock (_buildingXRef) _buildingXRef.TryGetValue(entityId, out topEntity);
                    break;

                case EntityLevel.Suite:
                    lock (_suiteXRef) _suiteXRef.TryGetValue(entityId, out topEntity);
                    break;
            }
    
            if (topEntity != null)
            {
                lock (_updateList)
                {
                    LinkedListNode<GenerationEntity> node = _updateList.Last;
                    while (node != null)
                    {
                        GenerationEntity e = node.Value;
                        
                        if (e.LastUpdateTime <= generationQueryValue) break;

                        if (e.Level >= topEntity.Level) // proper level node
                        {
                            // find root node of same level as in query
                            GenerationEntity re = e;
                            while (re.Level > topEntity.Level) re = re.Parent;  // "re" should never get null 
                            // as only developer-level nodes (least "Level" value) do not have parents

                            if (re.Id == topEntity.Id) // e is a subnode of requested element
                            {
                                if (0 == result.Generation)
                                {
                                    result.Generation = e.LastUpdateTime;
                                    if (0 == generationQueryValue) break;
                                }

                                switch (e.Level)
                                {
                                    case EntityLevel.Developer:
                                        if (null == result.Developers) result.Developers = new List<int>();
                                        result.Developers.Add(e.Id);
                                        break;

                                    case EntityLevel.Site:
                                        if (null == result.Sites) result.Sites = new List<int>();
                                        result.Sites.Add(e.Id);
                                        break;

                                    case EntityLevel.Building:
                                        if (null == result.Buildings) result.Buildings = new List<int>();
                                        result.Buildings.Add(e.Id);
                                        break;

                                    case EntityLevel.Suite:
                                        if (null == result.Suites) result.Suites = new List<int>();
                                        result.Suites.Add(e.Id);
                                        break;
                                }
                            }
                        }
                        node = node.Previous;
                    }  // node list loop
                }  // node list lock
            }  // entity found

            if (0 == result.Generation) result.Generation = generationQueryValue;  // no changes found; return same value

            if (0 == result.Generation) result.Generation = DateTime.UtcNow.AddMinutes(-5.0).Ticks;  // default value
            // in case no changes ever registered for this object tree

            return result;
        }

        private GenerationEntity getEntity(EstateDeveloper bo)
        {
            GenerationEntity result;
            lock (_developerXRef)
            {
                if (!_developerXRef.TryGetValue(bo.AutoID, out result))
                {
                    result = new GenerationEntity(EntityLevel.Developer, bo.AutoID, null, bo.Updated.Ticks);
                    _developerXRef.Add(bo.AutoID, result);
                }
            }
            return result;
        }

        private GenerationEntity getEntity(Site bo)
        {
            GenerationEntity result;
            lock (_siteXRef)
            {
                if (!_siteXRef.TryGetValue(bo.AutoID, out result))
                {
                    result = new GenerationEntity(EntityLevel.Site, bo.AutoID, getEntity(bo.Developer), bo.Updated.Ticks);
                    _siteXRef.Add(bo.AutoID, result);
                }
            }
            return result;
        }

        private GenerationEntity getEntity(Building bo)
        {
            GenerationEntity result;
            lock (_buildingXRef)
            {
                if (!_buildingXRef.TryGetValue(bo.AutoID, out result))
                {
                    result = new GenerationEntity(EntityLevel.Building, bo.AutoID, getEntity(bo.ConstructionSite), bo.Updated.Ticks);
                    _buildingXRef.Add(bo.AutoID, result);
                }
            }
            return result;
        }

        private GenerationEntity getEntity(Suite bo)
        {
            GenerationEntity result;
            lock (_suiteXRef)
            {
                if (!_suiteXRef.TryGetValue(bo.AutoID, out result))
                {
                    result = new GenerationEntity(EntityLevel.Suite, bo.AutoID, getEntity(bo.Building), bo.Updated.Ticks);
                    _suiteXRef.Add(bo.AutoID, result);
                }
            }
            return result;
        }

        public void Update(UpdateableBase bo)
        {
            GenerationEntity ge = null;

            if (bo is Suite) ge = getEntity(bo as Suite);
            else if (bo is Building) ge = getEntity(bo as Building);
            else if (bo is Site) ge = getEntity(bo as Site);
            else if (bo is EstateDeveloper) ge = getEntity(bo as EstateDeveloper);

            if (ge != null)
            {
                lock (_updateList)
                {
                    if (ge.ListNode != null) _updateList.Remove(ge.ListNode);
                    ge.LastUpdateTime = bo.Updated.Ticks;
                    ge.ListNode = _updateList.AddLast(ge);
                }
            }
        }
    }
}