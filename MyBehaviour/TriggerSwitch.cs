using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace DecorationMaster.MyBehaviour
{
    public class TriggerSwitch : MonoBehaviour
    {
        public delegate int IsKey(Collider2D key);
        public IsKey Check;
        private void OnTriggerEnter2D(Collider2D col)
        {
            if(Check != null)
            {
                Open(Check(col));
            }
        }
        public void Open(int gateNum = -1)
        {
            if (gateNum == -1)
                return;

            var gates = FindObjectsOfType<TriggerGate>().Where(x => x.GateNum == gateNum);
            foreach (var g in gates)
                g.Open();
        }
    }
    public class TriggerGate : MonoBehaviour
    {
        public int GateNum;
        public Action OpenGate;
        public void Open()
        {
            OpenGate?.Invoke();
        }
    }
}
