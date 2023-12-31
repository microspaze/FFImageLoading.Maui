﻿using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Sample
{
    public class DataUrlPageModel : ObservableObject
    {
        public DataUrlPageModel()
        {
        }

        public string SvgString => "<svg height=\"100\" width=\"100\"><circle cx=\"50\" cy=\"50\" r=\"40\" stroke=\"#000\" stroke-width=\"3\" fill=\"#ccc\"/></svg>";

        public string SvgStringUrlFormat => "data:image/svg+xml,<svg height=\"100\" width=\"100\"><circle cx=\"50\" cy=\"50\" r=\"40\" stroke=\"#000\" stroke-width=\"3\" fill=\"#ccc\"/></svg>";

        public void Reload()
        {
        }
    }
}
