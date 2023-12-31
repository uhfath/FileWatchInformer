﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileWatchInformer.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    internal sealed class RequiredFieldAttribute : RequiredAttribute
    {
        public RequiredFieldAttribute()
        {
            AllowEmptyStrings = false;
            ErrorMessageResourceType = typeof(Messages);
            ErrorMessageResourceName = nameof(Messages.RequiredFieldError);
        }
    }
}
