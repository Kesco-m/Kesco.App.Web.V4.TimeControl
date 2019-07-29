using System;
using System.Collections;
using System.Xml.Serialization;

namespace Kesco.App.Web.TimeControl.Entities
{
    [Serializable]
    public enum SortType
    {
        Asc = 0,
        Desc
    }

    [Serializable]
    public class SortInfoItem
    {
        public string ColumnName;
        public SortType ItemSortType;

        public SortInfoItem()
        {
        }

        public SortInfoItem(SortType sortType, string columnName)
        {
            ItemSortType = sortType;
            ColumnName = columnName;
        }
    }

    public class TimeInterval
    {
        public TimeInterval(DateTime pStartTime, DateTime pEndTime, int pPlaceCode)
        {
            StartTime = pStartTime;
            EndTime = pEndTime;
            PlaceCode = pPlaceCode;
        }

        public TimeInterval(DateTime pStartTime, DateTime pEndTime, int pPlaceCode, string pPlaceName)
        {
            StartTime = pStartTime;
            EndTime = pEndTime;
            PlaceCode = pPlaceCode;
            PlaceName = pPlaceName;
        }

        public TimeInterval(DateTime pStartTime, DateTime pEndTime, int pPlaceCode, bool startUndef, bool endUndef)
        {
            StartTime = pStartTime;
            EndTime = pEndTime;
            PlaceCode = pPlaceCode;

            StartUndefined = startUndef;
            EndUndefined = endUndef;
        }

        public TimeInterval(DateTime pStartTime, DateTime pEndTime, int pPlaceCode, bool startUndef, bool endUndef,
            string pPlaceName)
        {
            StartTime = pStartTime;
            EndTime = pEndTime;
            PlaceCode = pPlaceCode;

            StartUndefined = startUndef;
            EndUndefined = endUndef;
            PlaceName = pPlaceName;
        }

        public DateTime StartTime { get; }

        public DateTime EndTime { get; }

        public TimeSpan PeriodDuration => EndTime - StartTime;

        public int PlaceCode { get; }

        public string PlaceName { get; } = string.Empty;

        public bool StartUndefined { get; set; }

        public bool EndUndefined { get; set; }
    }

    /// <summary>
    ///     Summary description for EmployeeTimeInervals.
    /// </summary>
    public class EmployeeTimeInervals : CollectionBase
    {
        private static readonly Type t = typeof(TimeInterval);

        public TimeInterval this[int index]
        {
            get { return (TimeInterval) List[index]; }
            set { List[index] = value; }
        }

        public int Add(TimeInterval value)
        {
            return List.Add(value);
        }

        public int IndexOf(TimeInterval value)
        {
            return List.IndexOf(value);
        }

        public void Insert(int index, TimeInterval value)
        {
            List.Insert(index, value);
        }

        public void Remove(TimeInterval value)
        {
            List.Remove(value);
        }

        public bool Contains(TimeInterval value)
        {
            // If value is not of type Int16, this will return false.
            return List.Contains(value);
        }

        protected override void OnInsert(int index, object value)
        {
            if (value.GetType() != t)
                throw new ArgumentException("value must inherit from type " + t.FullName, "value");
        }

        protected override void OnRemove(int index, object value)
        {
            if (value.GetType() != t)
                throw new ArgumentException("value must inherit from type " + t.FullName, "value");
        }

        protected override void OnSet(int index, object oldValue, object newValue)
        {
            if (newValue.GetType() != t)
                throw new ArgumentException("newValue must inherit from type " + t.FullName, "newValue");
        }

        protected override void OnValidate(object value)
        {
            if (value.GetType() != t)
                throw new ArgumentException("value must inherit from type " + t.FullName);
        }
    }

    [Serializable]
    [XmlInclude(typeof(SortInfoItem))]
    public class SortInfoSetting : ArrayList
    {
        public string ToStringValue()
        {
            var result = string.Empty;
            foreach (SortInfoItem item in this)
            {
                result += item.ColumnName;
                result += ":";
                result += (int) item.ItemSortType;
                result += ",";
            }

            return result;
        }

        public void FromStringValue(string strValue)
        {
            Clear();
            var items = strValue.Split(',');
            foreach (var item in items)
                if (item != string.Empty)
                {
                    var parts = item.Split(':');
                    Add(new SortInfoItem((SortType) Enum.Parse(typeof(SortType), parts[1]), parts[0]));
                }
        }
    }
}