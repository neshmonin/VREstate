using System;
using System.Collections.Generic;

namespace Vre.Server.BusinessLogic
{
    [Serializable]
    public class ValueWithUM
    {
        protected const double FeetToMeters = 0.3048;
        protected const double MetersToFeet = 3.2808399;
        protected const double FeetToMetersSq = 0.09290304;
        protected const double MetersToFeetSq = 10.7639104167097223083335055559;

        public enum Unit : byte
        {
            Feet, Meters, SqFeet, SqMeters
        }

        protected double _rawValue;
        protected Unit _unit;

        public double ValueAs(Unit unit)
        {
            switch (_unit)
            {
                case Unit.Feet:
                    if (unit == Unit.Feet) return _rawValue;
                    else if (unit == Unit.Meters) return _rawValue * FeetToMeters;
                    else throw new ArgumentException("Cannot convert linear to area.");

                case Unit.Meters:
                    if (unit == Unit.Feet) return _rawValue * MetersToFeet;
                    else if (unit == Unit.Meters) return _rawValue;
                    else throw new ArgumentException("Cannot convert linear to area.");

                case Unit.SqFeet:
                    if (unit == Unit.SqFeet) return _rawValue;
                    else if (unit == Unit.SqMeters) return _rawValue * FeetToMetersSq;
                    else throw new ArgumentException("Cannot convert area to linear.");

                case Unit.SqMeters:
                    if (unit == Unit.SqFeet) return _rawValue * MetersToFeetSq;
                    else if (unit == Unit.SqMeters) return _rawValue;
                    else throw new ArgumentException("Cannot convert area to linear.");
            }

            throw new InvalidCastException("Unknown Unit of Measure.");
        }

        public void SetValue(double value, Unit unit)
        {
            switch (_unit)
            {
                case Unit.Feet:
                case Unit.Meters:
                    if ((unit == Unit.Feet) || (unit == Unit.Meters))
                    {
                        _rawValue = value;
                        _unit = unit;
                    }
                    else throw new ArgumentException("Cannot convert linear to area.");
                    break;

                case Unit.SqFeet:
                case Unit.SqMeters:
                    if ((unit == Unit.SqFeet) || (unit == Unit.SqMeters))
                    {
                        _rawValue = value;
                        _unit = unit;
                    }
                    else throw new ArgumentException("Cannot convert area to linear.");
                    break;
            }
        }

        public bool IsLinear { get { return ((_unit == Unit.Feet) || (_unit == Unit.Meters)); } }
        public bool IsArea { get { return ((_unit == Unit.SqFeet) || (_unit == Unit.SqMeters)); } }

        public Unit StoredAs { get { return _unit; } }

        public ValueWithUM(string raw)
        {
            string[] parts = raw.Split(',');
            if (parts.Length != 2) throw new ArgumentException("The formatted value passed is not valid.");
            try
            {
                _rawValue = double.Parse(parts[0]);
                _unit = (Unit)byte.Parse(parts[1]);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("The formatted value passed is not valid.", ex);
            }
        }

        public ValueWithUM(double value, Unit unit)
        {
            _rawValue = value;
            _unit = unit;
        }

        public string AsRaw
        {
            get
            {
                return string.Format("{0},{1}", _rawValue, (byte)_unit);
            }
        }

        public static ValueWithUM EmptyLinear { get { return new ValueWithUM(0.0, Unit.Meters); } }
        public static ValueWithUM EmptyArea { get { return new ValueWithUM(0.0, Unit.SqMeters); } }

        public override bool Equals(object obj)
        {
            ValueWithUM toCompare = obj as ValueWithUM;
            if (null == obj) return false;
            if (IsLinear && toCompare.IsLinear)
            {
                return ValueAs(Unit.Feet).Equals(toCompare.ValueAs(Unit.Feet));
            }
            else if (IsArea && toCompare.IsArea)
            {
                return ValueAs(Unit.SqFeet).Equals(toCompare.ValueAs(Unit.SqFeet));
            }
            return false;
        }
    }
}
