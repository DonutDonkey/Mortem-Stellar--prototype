﻿using System;
using System.Linq;
using Actor;
using Actor.Enemy;
using Data.Values;
using UnityEngine;


namespace Objects {
    public class Projectile : MonoBehaviour {
        [SerializeField] private Transform projectilePointTransform;
        
        [SerializeField] private FloatValue damageRadius;
        [SerializeField] private FloatValue damage;
        [SerializeField] private FloatValue speed;
        
        [SerializeField] private string particleName;
        [SerializeField] private string [] ignoreActorsDmg;

        private Rigidbody _rigidbody;

        private Vector3 _startingPosition;

        public Transform ProjectilePointTransform
        { get => projectilePointTransform; set => projectilePointTransform = value; }

        private void Awake() => _rigidbody = GetComponent<Rigidbody>();

        private void OnEnable() {
            _rigidbody.velocity = transform.forward * speed;
            _startingPosition = transform.position;
        }

        private void Update() {
            if( Vector3.Distance(transform.position, _startingPosition) > 100f)
                gameObject.SetActive(false);
        }

        private void OnCollisionEnter(Collision other) {
            Debug.Log("Projectile.OnCollisionEnter " + other.gameObject.name);
            
            SpawnImpactParticles(out var particleObj);
            particleObj.SetActive(true);

            var hit = Physics.OverlapSphere(transform.position, damageRadius);

            foreach (var variable in hit)
                DamageActorsIfHit(variable);
            
            gameObject.SetActive(false);
        }

        private void SpawnImpactParticles(out GameObject particleObj) {
            particleObj = ObjectPooler.SharedInstance.GetPooledObject(particleName);
            particleObj.transform.position = transform.position;
        }

        private void DamageActorsIfHit(Component inCollider) {
            var actor = inCollider.transform.GetComponent<ActorData>();

            if(actor == null) return;

            if (ignoreActorsDmg.Any(loopString => inCollider.gameObject.name == loopString)) return;

            if (actor is EnemyData enemy) {
                enemy.gameObject.GetComponent<Animator>().Play("Hurt");
                enemy.TakeDamage(GetFallowDamage(inCollider.transform), 
                    ProjectilePointTransform.GetComponentInParent<ActorData>());

                return;
            }

            actor.TakeDamage(GetFallowDamage(inCollider.transform));
        }

        private float GetFallowDamage(Transform other) => 
            (float) Math.Round(damage.value / GetDistance(other));

        private float GetDistance(Transform other) => 
            (float) Math.Round(Vector3.Distance(transform.position, other.transform.position));

        #region DebugInfo

        private void OnDrawGizmos() {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, damageRadius);
        }

        #endregion
    }
}
