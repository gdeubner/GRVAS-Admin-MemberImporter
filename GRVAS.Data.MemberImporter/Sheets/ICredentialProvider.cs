﻿using Google.Apis.Sheets.v4;

namespace GRVAS.Data.MemberImporter.Sheets;

internal interface ICredentialProvider
{
    SheetsService Service { get; set; }
}