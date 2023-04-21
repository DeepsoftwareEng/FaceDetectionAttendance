using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceDetectionAttendance.Model
{
    internal class LoginInfor
    {
       
        private string _myData;

        public string MyData
        {
            get { return _myData; }
            set
            {
                if (_myData != value)
                {
                    _myData = value;
                    OnPropertyChanged(nameof(MyData));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
