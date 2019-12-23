﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Data.Values;
using UnityEngine;

namespace Actor.Player.Weapons {
    [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
    public class ModelView : MonoBehaviour {
        [Header("Values")]
        
        [Tooltip("Reference for main fps camera")]
        [SerializeField] private Transform fpsCamera;

        [Tooltip("List of all weapon objects transforms")]
        [SerializeField] private List<Transform> weapons;
        +
        [SerializeField] private ModelViewOffset modelViewOffset;

        private Vector3 _offset;

        private void Awake() => _offset = modelViewOffset.offsetValue;

        private void LateUpdate () {
            _offset.x = (modelViewOffset.centerModelView)
                ? ModelViewOffset.DEFAULT_CENTERED_OFFSET_X
                : modelViewOffset.offsetValue.x;
            
            foreach (var t in weapons) 
                t.localPosition = fpsCamera.localPosition + _offset;
        }
        
        
    }
}