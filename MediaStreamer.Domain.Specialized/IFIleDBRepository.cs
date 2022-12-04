using System;
using System.Collections.Generic;
using System.Text;
using MediaStreamer.Domain;

namespace MediaStreamer.Domain.Specialized
{
    internal interface IFIleDBRepository : IDBRepository
    {
        new IFileDBContext DB { get; set; }
        bool AddCompositionBefore();
        bool AddCompositionAfter();
    }
}
