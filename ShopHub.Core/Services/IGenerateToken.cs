﻿using ShopHub.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopHub.Core.Services
{
    public interface IGenerateToken
    {
        string GetAndCreateToken(AppUser user);
    }
}
