﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.DTOS.AuthenticationDTO
{
    public class ConfirmEmailDTO
    {
        public string Email { get; set; }
        public string Code { get; set; }
    }
}
