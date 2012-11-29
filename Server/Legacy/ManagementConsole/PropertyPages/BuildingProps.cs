using System;
using System.Windows.Forms;
using NHibernate;
using Vre.Server.BusinessLogic;
using Vre.Server.Dao;

namespace Vre.Server.ManagementConsole
{
    public partial class BuildingProps : UserControl, IPropertyPage
    {
        private Building _object;

        public BuildingProps()
        {
            InitializeComponent();
            
            cbxStatus.Items.Add("In Project");
            cbxStatus.Items.Add("Constructing");
            cbxStatus.Items.Add("Built");
            cbxStatus.Items.Add("Sold");

            dtpOpeningDate.MinDate = NHibernateHelper.DateTimeMinValue;
            //dtpOpeningDate.MaxDate = NHibernateHelper.DateTimeMaxValue;
        }

        private int statusToIndex(Building.BuildingStatus status)
        {
            switch (status)
            {
                case Building.BuildingStatus.InProject:
                    return 0;

                case Building.BuildingStatus.Constructing:
                    return 1;

                case Building.BuildingStatus.Built:
                    return 2;

                case Building.BuildingStatus.Sold:
                    return 3;

                default:
                    throw new ApplicationException("Unknown status value.");
            }
        }

        private Building.BuildingStatus indexToStatus(int index)
        {
            switch (index)
            {
                case 0:
                    return Building.BuildingStatus.InProject;

                case 1:
                    return Building.BuildingStatus.Constructing;

                case 2:
                    return Building.BuildingStatus.Built;

                case 3:
                    return Building.BuildingStatus.Sold;

                default:
                    throw new ApplicationException("Unknown index value.");
            }
        }

        public void SetObject(object obj)
        {
            _object = obj as Building;

            if (_object != null)
            {
                tbName.Text = _object.Name;
                cbxStatus.SelectedIndex = statusToIndex(_object.Status);
                PropertyPageHelper.NullableDateTimeToControl(_object.OpeningDate, dtpOpeningDate);
                tbAddressLine1.Text = _object.AddressLine1;
                tbAddressLine2.Text = _object.AddressLine2;
                tbCity.Text = _object.City;
                tbStateProvince.Text = _object.StateProvince;
                tbPostalCode.Text = _object.PostalCode;
                tbCountry.Text = _object.Country;
            }
            else
            {
                tbName.Text = "New Building";
                cbxStatus.SelectedIndex = 0;
                dtpOpeningDate.Checked = false;
                tbAddressLine1.Text = string.Empty;
                tbAddressLine2.Text = string.Empty;
                tbCity.Text = string.Empty;
                tbStateProvince.Text = string.Empty;
                tbPostalCode.Text = string.Empty;
                tbCountry.Text = string.Empty;
            }
        }

        public string ObjectName { get { return "Building"; } }

        public bool Changed
        {
            get
            {
                if (_object != null)
                {
                    return (
                        (!PropertyPageHelper.NullableStringsAreEqual(tbName.Text, _object.Name)) ||
                        (!indexToStatus(cbxStatus.SelectedIndex).Equals(_object.Status)) ||
                        (!PropertyPageHelper.CompareDTP2NDT(dtpOpeningDate, _object.OpeningDate)) ||
                        (!PropertyPageHelper.NullableStringsAreEqual(tbAddressLine1.Text, _object.AddressLine1)) ||
                        (!PropertyPageHelper.NullableStringsAreEqual(tbAddressLine2.Text, _object.AddressLine2)) ||
                        (!PropertyPageHelper.NullableStringsAreEqual(tbCity.Text, _object.City)) ||
                        (!PropertyPageHelper.NullableStringsAreEqual(tbStateProvince.Text, _object.StateProvince)) ||
                        (!PropertyPageHelper.NullableStringsAreEqual(tbPostalCode.Text, _object.PostalCode)) ||
                        (!PropertyPageHelper.NullableStringsAreEqual(tbCountry.Text, _object.Country))
                        );
                }
                else
                    return true;
            }
        }

        public void SetupNode(TreeNode node)
        {
            SetupNode(node, _object);
        }

        public void SetupNode(TreeNode node, Building obj)
        {
            if (obj != null) node.Text = obj.ToString();
            node.Tag = obj;
        }

        public void Save(object parent, ISession session)
        {
            if (null == _object)
            {
                Site s = parent as Site;
                if (null == s) throw new ApplicationException("Parent object is invalid.");
                _object = new Building(s, tbName.Text);
            }
            else
            {
                _object.Name = tbName.Text;
            }

            _object.Status = indexToStatus(cbxStatus.SelectedIndex);
            _object.OpeningDate = PropertyPageHelper.ControlToNullableDateTime(dtpOpeningDate);
            _object.AddressLine1 = tbAddressLine1.Text;
            _object.AddressLine2 = tbAddressLine2.Text;
            _object.City = tbCity.Text;
            _object.StateProvince = tbStateProvince.Text;
            _object.PostalCode = tbPostalCode.Text;
            _object.Country = tbCountry.Text;

            using (BuildingDao dao = new BuildingDao(session)) dao.CreateOrUpdate(_object);
        }
    }
}
