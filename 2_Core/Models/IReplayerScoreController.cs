﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatLeader.Models
{
    public interface IReplayerScoreController : IScoreController
    {
        int maxComboAfterRescoring { get; }
        int comboAfterRescoring { get; }

        event Action<int, int, bool> onComboChangedAfterRescoring;
    }
}
