﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Dtos.GenreOffer
{
    public class GenreResponse
    {
        public Guid GenreID { get; set; }
        public string GenreName { get; set; }
        public string GenreOfferImgURL { get; set; }
    }
}
