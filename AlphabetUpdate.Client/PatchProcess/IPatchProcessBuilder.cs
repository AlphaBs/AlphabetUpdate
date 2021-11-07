﻿using System.Threading.Tasks;
using AlphabetUpdate.Client.PatchHandler;

namespace AlphabetUpdate.Client.PatchProcess
{
    public interface IPatchProcessBuilder
    {
        Task<PatchProcess> Build();
    }
}