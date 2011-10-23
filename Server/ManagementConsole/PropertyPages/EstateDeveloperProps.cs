using System;
using System.Windows.Forms;
using NHibernate;
using Vre.Server.BusinessLogic;
using Vre.Server.Dao;

namespace Vre.Server.ManagementConsole
{
    public partial class EstateDeveloperProps : UserControl, IPropertyPage
    {
        private EstateDeveloper _object;

        public EstateDeveloperProps()
        {
            InitializeComponent();

            cbxMode.Items.Add("Kiosk, Single Screen");
            cbxMode.Items.Add("Kiosk, Single Office, Multiple Screens");
            cbxMode.Items.Add("Kiosk, Multiple Offices");
            cbxMode.Items.Add("Online");
        }

        private int configurationToIndex(EstateDeveloper.Configuration cfg)
        {
            switch (cfg)
            {
                case EstateDeveloper.Configuration.Kiosk_SingleScreen:
                    return 0;

                case EstateDeveloper.Configuration.Kiosk_SingleOffice_MultipleScreens:
                    return 1;

                case EstateDeveloper.Configuration.Kiosk_MultipleOffices:
                    return 2;

                case EstateDeveloper.Configuration.Online:
                    return 3;

                default:
                    throw new ApplicationException("Unknown configuration value.");
            }
        }

        private EstateDeveloper.Configuration indexToConfiguration(int index)
        {
            switch (index)
            {
                case 0:
                    return EstateDeveloper.Configuration.Kiosk_SingleScreen;

                case 1:
                    return EstateDeveloper.Configuration.Kiosk_SingleOffice_MultipleScreens;

                case 2:
                    return EstateDeveloper.Configuration.Kiosk_MultipleOffices;

                case 3:
                    return EstateDeveloper.Configuration.Online;

                default:
                    throw new ApplicationException("Unknown index value.");
            }
        }

        public void SetObject(object obj)
        {
            _object = obj as EstateDeveloper;

            if (_object != null)
            {
                tbName.Text = _object.Name;
                cbxMode.SelectedIndex = configurationToIndex(_object.VREConfiguration);
                cbxMode.Enabled = false;
            }
            else
            {
                tbName.Text = "New Developer";
                cbxMode.SelectedIndex = 0;
                cbxMode.Enabled = true;
            }
        }

        public string ObjectName { get { return "Estate Developer"; } }

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

        public void SetupNode(TreeNode node, EstateDeveloper obj)
        {
            if (obj != null) node.Text = obj.ToString();
            node.Tag = obj;
        }

        public void Save(object parent, ISession session)
        {
            if (null == _object)
            {
                _object = new EstateDeveloper(indexToConfiguration(cbxMode.SelectedIndex));
            }

            _object.Name = tbName.Text;

            using (EstateDeveloperDao dao = new EstateDeveloperDao(session)) dao.CreateOrUpdate(_object);
        }
    }
}
