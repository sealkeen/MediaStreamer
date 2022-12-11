using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace MediaStreamer.RAMControl
{
    public class MainPageViewModel
    {
        private int _skipRecordsCount { get; set; } = 0;
        private int _takeRecordsCount { get; set; } = 30;

        public Action UpdateBindingExpression {get; set; }

        public void SetSkip(int skip, Action<string> log = null) { _skipRecordsCount = skip; RecordEnumerator = $"{skip}"; log($"Set skip {skip};"); }
        public void SetTake(int take, Action<string> log = null) { _takeRecordsCount = take; RecordEnumerator = $"{take}"; log($"Set take {take};"); }
        public int GetSkip() => _skipRecordsCount;
        public int GetTake() => _takeRecordsCount;

        public String RecordEnumerator
        {
            get { return $"Fetched {_skipRecordsCount} / {_takeRecordsCount}"; }
            set { NotifyPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
