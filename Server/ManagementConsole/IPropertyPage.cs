using NHibernate;
using System.Windows.Forms;
using Vre.Server.BusinessLogic;

namespace Vre.Server.ManagementConsole
{
    //public interface IPropertyPage<T> where T : UpdateableBase
    public interface IPropertyPage
    {
        string ObjectName { get; }
        //void SetObject(T obj);
        void SetObject(object obj);
        void SetupNode(TreeNode node);
        //void SetupNode(TreeNode node, T obj);
        bool Changed { get; }
        void Save(object parent, ISession session);
    }
}