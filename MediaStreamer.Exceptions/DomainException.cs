using System;
using System.Collections.Generic;
using System.Text;

namespace MediaStreamer.Exceptions
{
    public class DomainException : ArgumentException
    {
        public DomainException(string message) : base(message) 
        {
            
        }
    }
}
