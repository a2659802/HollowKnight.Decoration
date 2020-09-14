﻿using System;
using System.Collections.Generic;
using Modding;
using UnityEngine;

namespace DecorationMaster
{
    [Serializable]
    public class SaveModSettings : ModSettings
    {

    }

    [Serializable]
    public class GlobalModSettings : ModSettings
    {
        public List<Item> items = new List<Item>();
    }
    public enum Operation
    {
        None, // DataBase
        ADD,
        REMOVE,
        COPY,

        Serialize, // Default
        SetPos,

        SetRot, // Resizable
        SetSize,

        SetSpan, // Saw
        SetSpeed,
        SetTinkVoice,
        SetOffset,
    }
    
    [Serializable]
    public struct V2
    {
        public float x;
        public float y;
        public V2(Vector2 uv2)
        {
            x = uv2.x;
            y = uv2.y;
        }
        public V2(float x,float y)
        {
            this.x = x;
            this.y = y;
        }
        public static implicit operator Vector2 (V2 v2)
        {
            Vector2 uv2 = new Vector2(v2.x, v2.y);
            return uv2;
        }
        public static implicit operator Vector3(V2 v2)
        {
            Vector3 uv2 = new Vector2(v2.x, v2.y);
            return uv2;
        }
        public static implicit operator V2(Vector2 uv2)
        {
            V2 v2 = new V2(uv2);
            return v2;
        }
        public static implicit operator V2(Vector3 uv3)
        {
            V2 v2 = new V2(uv3.x, uv3.y);
            return v2;
        }
    }
}
