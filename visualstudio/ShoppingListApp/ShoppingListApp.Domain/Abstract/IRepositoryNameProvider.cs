﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingListApp.Domain.Abstract
{
    public interface IRepositoryNameProvider
    {
        string repositoryName { get; }
    }
}