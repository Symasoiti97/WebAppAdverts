using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.Services
{
    public interface IConverterService<U,V>
    {
        U Convert(V obj);
    }
}
