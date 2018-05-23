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
        public SortType ItemSortType;
        public string ColumnName;

        public SortInfoItem() { }

        public SortInfoItem(SortType sortType, string columnName)
        {
            ItemSortType = sortType;
            ColumnName = columnName;
        }
    }

    public class TimeInterval
    {
        private DateTime startTime;
        private DateTime endTime;
        private int placeCode;
        private bool startUndefined = false;
        private bool endUndefined = false;
        private string placeName = String.Empty;

        public DateTime StartTime
        {
            get { return startTime; }
        }

        public DateTime EndTime
        {
            get { return endTime; }
        }

        public TimeSpan PeriodDuration
        {
            get { return endTime - startTime; }
        }

        public int PlaceCode
        {
            get { return placeCode; }
        }

        public string PlaceName
        {
            get { return placeName; }
        }

        public bool StartUndefined
        {
            get { return startUndefined; }
            set { startUndefined = value; }
        }

        public bool EndUndefined
        {
            get { return endUndefined; }
            set { endUndefined = value; }
        }

        public TimeInterval(DateTime pStartTime, DateTime pEndTime, int pPlaceCode)
        {
            startTime = pStartTime;
            endTime = pEndTime;
            placeCode = pPlaceCode;
        }

        public TimeInterval(DateTime pStartTime, DateTime pEndTime, int pPlaceCode, string pPlaceName)
        {
            startTime = pStartTime;
            endTime = pEndTime;
            placeCode = pPlaceCode;
            placeName = pPlaceName;
        }

        public TimeInterval(DateTime pStartTime, DateTime pEndTime, int pPlaceCode, bool startUndef, bool endUndef)
        {
            startTime = pStartTime;
            endTime = pEndTime;
            placeCode = pPlaceCode;

            startUndefined = startUndef;
            endUndefined = endUndef;
        }

        public TimeInterval(DateTime pStartTime, DateTime pEndTime, int pPlaceCode, bool startUndef, bool endUndef, string pPlaceName)
        {
            startTime = pStartTime;
            endTime = pEndTime;
            placeCode = pPlaceCode;

            startUndefined = startUndef;
            endUndefined = endUndef;
            placeName = pPlaceName;
        }
    }

    /// <summary>
    /// Summary description for EmployeeTimeInervals.
    /// </summary>
    public class EmployeeTimeInervals : CollectionBase
    {
        static Type t = typeof(TimeInterval);

        public TimeInterval this[int index]
        {
            get
            {
                return ((TimeInterval)List[index]);
            }
            set
            {
                List[index] = value;
            }
        }

        public int Add(TimeInterval value)
        {
            return (List.Add(value));
        }

        public int IndexOf(TimeInterval value)
        {
            return (List.IndexOf(value));
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
            return (List.Contains(value));
        }

        protected override void OnInsert(int index, Object value)
        {
            if (value.GetType() != t)
                throw new ArgumentException("value must inherit from type " + t.FullName, "value");
        }

        protected override void OnRemove(int index, Object value)
        {
            if (value.GetType() != t)
                throw new ArgumentException("value must inherit from type " + t.FullName, "value");
        }

        protected override void OnSet(int index, Object oldValue, Object newValue)
        {
            if (newValue.GetType() != t)
                throw new ArgumentException("newValue must inherit from type " + t.FullName, "newValue");
        }

        protected override void OnValidate(Object value)
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
            string result = String.Empty;
            foreach (SortInfoItem item in this)
            {
                result += item.ColumnName;
                result += ":";
                result += (int)item.ItemSortType;
                result += ",";
            }
            return result;
        }

        public void FromStringValue(string strValue)
        {
            Clear();
            string[] items = strValue.Split(',');
            foreach (string item in items)
            {
                if (item != String.Empty)
                {
                    string[] parts = item.Split(':');
                    Add(new SortInfoItem((SortType)Enum.Parse(typeof(SortType), parts[1]), parts[0]));
                }
            }
        }
    }
}