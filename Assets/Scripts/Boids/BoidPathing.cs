using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AI
{
    public static class BoidPathing
    {
        public static float PathingResolution = 0.75f;
        public static float SeparationDistance = 4f;
        public static float DistanceScale = 10f;

        /// <summary>
        /// Calculates and returns an adjusted path adhering to flocking behaviour.
        /// </summary>
        /// <param name="ai"></param>
        /// <param name="target"></param>
        /// <param name="targetPath"></param>
        /// <param name="flock"></param>
        /// <param name="userboid"></param>
        /// <returns></returns>
        public static Tuple<bool, bool, Vector3> AdjustPath_Flock(Vector3 target, FlockController flock, IFlockable userboid)
        {
            Entity ai = userboid.EntitySelf;
            bool arrived = false;
            bool flockArrived = false;
            Vector3 adjustedPoint = new Vector3();

            NavMeshPath targetPath = new NavMeshPath();
            NavMesh.CalculatePath(ai.transform.position, target, NavMesh.AllAreas, targetPath);

            if (targetPath.corners.Length > 1)
            {
                if (Vector3.Distance(ai.transform.position, targetPath.corners[1]) > 2 && flock != null)
                {
                    //Adjust first corner
                    adjustedPoint = CalculateAdjustment(ai, targetPath.corners[1], flock);
                }
                else if (Vector3.Distance(ai.transform.position, target) < Mathf.Pow(flock.FlockMembers.Count, 1 / 3) * DistanceScale)
                {
                    arrived = true;
                    if (TestFlock(flock, userboid))
                    {
                        flockArrived = true;
                    }
                }
                else if (targetPath.corners.Length > 2 && flock != null)
                {
                    //Adjust second corner
                    adjustedPoint = CalculateAdjustment(ai, targetPath.corners[2], flock);
                }
            }
            return new Tuple<bool, bool, Vector3>(arrived, flockArrived, adjustedPoint);
        }
        private static Vector3 CalculateAdjustment(Entity ai, Vector3 point, FlockController flock)
        {
            Vector3 pos = ai.transform.position;
            Vector3 Alignment = (point - pos).normalized;
            Vector3 Cohesion = pos;
            Vector3 Separation = Vector3.zero;

            int boidCount = flock.FlockMembers.Count;

            Collider[] _ = Physics.OverlapSphere(pos, 10);
            List<GameObject> NearbyObjs = new List<GameObject>();

            for (int i = 0; i < _.Length; i++)
            {
                NearbyObjs.Add(_[i].gameObject);
            }

            foreach (IFlockable boid in flock.FlockMembers)
            {
                if (boid.EntitySelf.gameObject == ai.gameObject || !NearbyObjs.Contains(boid.EntitySelf.gameObject)) continue;

                Transform _t = boid.EntitySelf.transform;

                Cohesion += _t.position;
                Separation += GetSeparationVector(ai, _t);
            }

            if (boidCount > 1)
            {
                float Average = 1.0f / (boidCount);
                Cohesion *= Average;
            }
            Cohesion = (Cohesion - pos);

            Vector3 AdjustedHeading = (Alignment * 4 + Separation + Cohesion).normalized;
            Debug.DrawRay(ai.transform.position, Alignment * (point - pos).magnitude, Color.magenta, 0.5f);
            Debug.DrawRay(ai.transform.position, Separation, Color.white, 0.5f);
            Debug.DrawRay(ai.transform.position, Cohesion, Color.blue, 0.5f);
            return pos + (AdjustedHeading * (point - pos).magnitude);
        }

        private static Vector3 GetSeparationVector(Entity ai, Transform target)
        {
            Vector3 Opposite = ai.transform.position - target.position;
            float OppositeMag = Opposite.magnitude;
            float ScaledDir = Mathf.Clamp01(1.0f - OppositeMag / SeparationDistance);
            return Opposite.normalized * ScaledDir;
        }

        private static bool TestFlock(FlockController flock, IFlockable userBoid)
        {

            foreach (IFlockable Boid in flock.FlockMembers)
            {
                if (Boid == userBoid) continue;

                if (Boid.Arrived == false)
                {
                    return false;
                }
            }
            return true;
        }
    }

}