﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimForm_Importer
{
    public interface IDatabaseClient
    {

        string Url { get; set; }
        string Database { get; set; }

    }
}