﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Xemio.Client.Data.Endpoints.Notebooks
{
    public class CreateNotebookAction
    {
        public string Name { get; set; }
        public string ParentNotebookId { get; set; }
    }
}
