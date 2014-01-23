using System;

namespace Vre.Server.BusinessLogic
{
    internal class ServiceRequestItem : UpdateableBase
    {
        public enum ServiceType { ViewOrder }

        public Invoice.SubjectType TargetObjectType { get; private set; }
        public int? TargetObjectId { get; private set; }
        public Guid? TargetObjectGuid { get; private set; }
        public decimal Price { get; private set; }

        public ServiceType ServiceObjectType { get; private set; }
        public int ServiceObjectId { get; private set; }

		private ServiceRequestItem() { }

        public ServiceRequestItem(Invoice.SubjectType @object, int objectId, ServiceType subject, int subjectId, decimal price)
            : base()
        {
			InitializeNew();
            TargetObjectType = @object;
            TargetObjectId = objectId;
            TargetObjectGuid = null;
            ServiceObjectType = subject;
            ServiceObjectId = subjectId;
            Price = price;
        }
    }
}
