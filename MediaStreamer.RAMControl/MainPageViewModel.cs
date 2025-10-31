using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MediaStreamer.RAMControl
{
    public class MainPageViewModel
    {
        private int _skipRecordsCount { get; set; } = 0;
        private int _takeRecordsCount { get; set; } = 30;
        private int _totalRecords { get; set; } = 0; 

        public Action UpdateBindingExpression { get; set; }

        public void SetTotal(int total, Action<string> log = null) { _totalRecords = total; RecordEnumerator = $"{total}"; log?.Invoke($"Set total {total};"); }
        public void SetSkip(int skip, Action<string> log = null) { _skipRecordsCount = skip; RecordEnumerator = $"{skip}"; log?.Invoke($"Set skip {skip};"); }
        public void SetTake(int take, Action<string> log = null) { _takeRecordsCount = take; RecordEnumerator = $"{take}"; log?.Invoke($"Set take {take};"); }
        public int GetSkip() => _skipRecordsCount;
        public int GetTake() => _takeRecordsCount;
        public int GetLastPageIndex() => (int)Math.Round((double)_totalRecords / _takeRecordsCount, 2);

        public String RecordEnumerator
        {
            get { return $"Fetched {Math.Round((double)_skipRecordsCount / _takeRecordsCount, 2)} / {Math.Round((double)_totalRecords / _takeRecordsCount, 2)}"; }
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
