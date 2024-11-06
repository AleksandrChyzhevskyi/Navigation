using System;
using UnityEngine;

namespace _Development.Scripts.Navigation
{
    public class LevelPoint : MonoBehaviour
    {
        public LevelNumber NameLevelPoint;
        public TargetPoint Target;
        
        [field: SerializeField] public bool IsCompleted { get; private set; }

        public int PriorityIndex => (int)NameLevelPoint;

        public void SetStateCompleted(bool state) =>
            IsCompleted = state;
    }
}