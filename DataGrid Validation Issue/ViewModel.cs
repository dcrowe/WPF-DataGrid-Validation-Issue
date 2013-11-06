using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGrid_Validation_Issue
{
    public class ViewModel
    {
        public ViewModel()
        {
            Items = new ObservableCollection<Item>
            {
                new Item { Name = "Bob" },
                new Item { Name = "Jane" },
                new Item { Name = "Tanya" }
            };
        }

        public ObservableCollection<Item> Items { get; private set; }
    }

    public class Item : INotifyDataErrorInfo, INotifyPropertyChanged
    {
        Dictionary<string, IEnumerable<string>> _errors = new Dictionary<string, IEnumerable<string>>();
        string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    ValidateProperty("Name", value);
                    _name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string p)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(p));
        }

        private void ValidateProperty(string p, object value)
        {
            if (p == "Name")
            {
                if (string.IsNullOrWhiteSpace((string)value))
                    _errors["Name"] = new[] { "Name is required." };
                else if (_errors.ContainsKey("Name"))
                    _errors.Remove("Name");
            }

            if (ErrorsChanged != null)
                ErrorsChanged(this, new DataErrorsChangedEventArgs(null));
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public System.Collections.IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                return _errors.Values.SelectMany(es2 => es2);

            IEnumerable<string> es;
            _errors.TryGetValue(propertyName ?? "", out es);
            return es;
        }

        public bool HasErrors
        {
            get
            {
                var e = _errors.Values.Any(es => es.Any());
                return e;
            }
        }
    }

    public class ViewModel2
    {
        public ViewModel2()
        {
            Items = new ObservableCollection<Item2>
            {
                new Item2 { Name = "Bob" },
                new Item2 { Name = "Jane" },
                new Item2 { Name = "Tanya" }
            };
        }

        public ObservableCollection<Item2> Items { get; private set; }
    }

    public class Item2 : IDataErrorInfo, INotifyPropertyChanged
    {
        Dictionary<string, IEnumerable<string>> _errors = new Dictionary<string, IEnumerable<string>>();
        string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    ValidateProperty("Name", value);
                    _name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string p)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(p));
        }

        private void ValidateProperty(string p, object value)
        {
            if (p == "Name")
            {
                if (string.IsNullOrWhiteSpace((string)value))
                    _errors["Name"] = new[] { "Name is required." };
                else
                    _errors["Name"] = new string[0];
            }
        }

        string IDataErrorInfo.Error
        {
            get { return string.Join(", ", _errors.Values.SelectMany(e => e)); }
        }

        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                IEnumerable<string> es;
                _errors.TryGetValue(columnName, out es);

                return es == null ? null : string.Join(", ", es);
            }
        }
    }
}
