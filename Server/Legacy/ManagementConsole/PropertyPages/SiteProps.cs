using System;
using System.Windows.Forms;
using NHibernate;
using Vre.Server.BusinessLogic;
using Vre.Server.Dao;

namespace Vre.Server.ManagementConsole
{
    public partial class SiteProps : UserControl, IPropertyPage
    {
        private Site _object;

        public SiteProps()
        {
            InitializeComponent();
        }

        public void SetObject(object obj)
        {
            _object = obj as Site;

            if (_object != null)
            {
                tbName.Text = _object.Name;
            }
            else
            {
                tbName.Text = "New Site";
            }
        }

        public string ObjectName { get { return "Construction Site"; } }

        public bool Changed
        {
            get
            {
                if (_object != null)
                    return (!_object.Name.Equals(tbName.Text));
                else
                    return true;
            }
        }

        public void SetupNode(TreeNode node)
        {
            SetupNode(node, _object);
        }

        public void SetupNode(TreeNode node, Site obj)
        {
            if (obj != null) node.Text = obj.ToString();
            node.Tag = obj;
        }

        public void Save(object parent, ISession session)
        {
            if (null == _object)
            {
                EstateDeveloper ed = parent as EstateDeveloper;
                if (null == ed) throw new ApplicationException("Parent object is invalid.");
                _object = new Site(ed, tbName.Text);
            }
            else
            {
                _object.Name = tbName.Text;
            }

            using (SiteDao dao = new SiteDao(session)) dao.CreateOrUpdate(_object);
        }
    }
}
