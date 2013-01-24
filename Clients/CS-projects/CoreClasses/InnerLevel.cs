using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreClasses
{
    public class InnerLevel : IComparable
    {
        public enum Category
        {
            Basement,
            SemiBasement,
            Main,
            MainSplit,
            Second,
            SecondSplit,
            Third,
            ThirdSplit,
            Fourth,
            Attic
        }

        public Category category = InnerLevel.Category.Main;
        public List<Room> Rooms = new List<Room>();

        public InnerLevel(InnerLevel.Category cat)
        {
            category = cat;
        }

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            InnerLevel level = obj as InnerLevel;

            switch (level.category)
            {
                case Category.Basement:
                    switch (category)
                    {
                        case Category.Basement: return 0;
                        case Category.SemiBasement: return 1;
                        case Category.Main: return 1;
                        case Category.MainSplit: return 1;
                        case Category.Second: return 1;
                        case Category.SecondSplit: return 1;
                        case Category.Third: return 1;
                        case Category.ThirdSplit: return 1;
                        case Category.Fourth: return 1;
                        case Category.Attic: return 1;
                    }
                    break;
                case Category.SemiBasement:
                    switch (category)
                    {
                        case Category.Basement: return -1;
                        case Category.SemiBasement: return 0;
                        case Category.Main: return 1;
                        case Category.MainSplit: return 1;
                        case Category.Second: return 1;
                        case Category.SecondSplit: return 1;
                        case Category.Third: return 1;
                        case Category.ThirdSplit: return 1;
                        case Category.Fourth: return 1;
                        case Category.Attic: return 1;
                    }
                    break;
                case Category.Main:
                    switch (category)
                    {
                        case Category.SemiBasement: return -1;
                        case Category.Basement: return -1;
                        case Category.Main: return 0;
                        case Category.MainSplit: return 1;
                        case Category.Second: return 1;
                        case Category.SecondSplit: return 1;
                        case Category.Third: return 1;
                        case Category.ThirdSplit: return 1;
                        case Category.Fourth: return 1;
                        case Category.Attic: return 1;
                    }
                    break;
                case Category.MainSplit:
                    switch (category)
                    {
                        case Category.SemiBasement: return -1;
                        case Category.Basement: return -1;
                        case Category.Main: return -1;
                        case Category.MainSplit: return 0;
                        case Category.Second: return 1;
                        case Category.SecondSplit: return 1;
                        case Category.Third: return 1;
                        case Category.ThirdSplit: return 1;
                        case Category.Fourth: return 1;
                        case Category.Attic: return 1;
                    }
                    break;
                case Category.Second:
                    switch (category)
                    {
                        case Category.SemiBasement: return -1;
                        case Category.Basement: return -1;
                        case Category.Main: return -1;
                        case Category.MainSplit: return -1;
                        case Category.Second: return 0;
                        case Category.SecondSplit: return 1;
                        case Category.Third: return 1;
                        case Category.ThirdSplit: return 1;
                        case Category.Fourth: return 1;
                        case Category.Attic: return 1;
                    }
                    break;
                case Category.SecondSplit:
                    switch (category)
                    {
                        case Category.SemiBasement: return -1;
                        case Category.Basement: return -1;
                        case Category.Main: return -1;
                        case Category.MainSplit: return -1;
                        case Category.Second: return -1;
                        case Category.SecondSplit: return 0;
                        case Category.Third: return 1;
                        case Category.ThirdSplit: return 1;
                        case Category.Fourth: return 1;
                        case Category.Attic: return 1;
                    }
                    break;
                case Category.Third:
                    switch (category)
                    {
                        case Category.SemiBasement: return -1;
                        case Category.Basement: return -1;
                        case Category.Main: return -1;
                        case Category.MainSplit: return -1;
                        case Category.Second: return -1;
                        case Category.SecondSplit: return -1;
                        case Category.Third: return 0;
                        case Category.ThirdSplit: return 1;
                        case Category.Fourth: return 1;
                        case Category.Attic: return 1;
                    }
                    break;
                case Category.ThirdSplit:
                    switch (category)
                    {
                        case Category.SemiBasement: return -1;
                        case Category.Basement: return -1;
                        case Category.Main: return -1;
                        case Category.MainSplit: return -1;
                        case Category.Second: return -1;
                        case Category.SecondSplit: return -1;
                        case Category.Third: return -1;
                        case Category.ThirdSplit: return 0;
                        case Category.Fourth: return 1;
                        case Category.Attic: return 1;
                    }
                    break;
                case Category.Fourth:
                    switch (category)
                    {
                        case Category.SemiBasement: return -1;
                        case Category.Basement: return -1;
                        case Category.Main: return -1;
                        case Category.MainSplit: return -1;
                        case Category.Second: return -1;
                        case Category.SecondSplit: return -1;
                        case Category.Third: return -1;
                        case Category.ThirdSplit: return -1;
                        case Category.Fourth: return 0;
                        case Category.Attic: return 1;
                    }
                    break;
                case Category.Attic:
                    switch (category)
                    {
                        case Category.SemiBasement: return -1;
                        case Category.Basement: return -1;
                        case Category.Main: return -1;
                        case Category.MainSplit: return -1;
                        case Category.Second: return -1;
                        case Category.SecondSplit: return -1;
                        case Category.Third: return -1;
                        case Category.ThirdSplit: return -1;
                        case Category.Fourth: return -1;
                        case Category.Attic: return 0;
                    }
                    break;
            }
            throw new NotImplementedException();
        }

        #endregion
    }
}
