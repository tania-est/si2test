﻿using System;
using System.Collections.Generic;
using System.Text;

namespace si2.bll.Dtos.Results.Administration
{
    public class UserClaimsDto
    {
        public UserClaimsDto()
        {
            Claims = new List<UserClaimDto>();
        }

        public List<UserClaimDto> Claims { get; set; }
    }
}
