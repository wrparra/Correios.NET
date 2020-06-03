using Correios.NET.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Correios.NET.Models
{
    [Flags]
    public enum DeliveryOptions
    {
        [DescriptionAndValue("PAC", "04510")]
        PAC = 1 << 0,
        
        [DescriptionAndValue("SEDEX", "04014")]
        SEDEX = 1 << 1,

        [DescriptionAndValue("SEDEX 10", "40215")]
        SEDEX_10 = 1 << 2,

        [DescriptionAndValue("SEDEX 12", "40169")]
        SEDEX_12 = 1 << 3,

        [DescriptionAndValue("SEDEX HOJE", "40290")]
        SEDEX_HOJE = 1 << 4

    }
}
