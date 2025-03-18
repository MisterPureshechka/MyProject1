using System;
using System.Collections.Generic;
using Scripts.Metadata;
using Scripts.Utils;
using Newtonsoft.Json;
using Scripts.Metadata.Knowledge;

namespace Scripts.Progress
{
    [Serializable]
    public class ProgressData
    {
        public Dictionary<string, Metadata.Metadata> Metadata = new();
        
        public ProgressData() 
        {
            Metadata[Consts.Food] = new Food();
            Metadata[Consts.Energy] = new Energy();
            Metadata[Consts.Shower] = new Shower();

            Metadata[Consts.Programming] = new Programming();
            Metadata[Consts.Art] = new Art();
            Metadata[Consts.Marketing] = new Marketing();
            Metadata[Consts.GameDesign] = new GameDesign();
        }
    }
}