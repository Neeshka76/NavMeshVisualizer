using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;

namespace NavMeshVisualizer
{
    public class SpellNavMeshVisualizer : SpellCastCharge
    {
        private static NavMeshVisual meshVisual;
        private Vector3 direction;
        private bool isCasting;
        private float strenghtOfMvt = 1.5f;

        public override void Fire(bool active)
        {
            base.Fire(active);
            isCasting = active;
        }

        public override void UpdateCaster()
        {
            base.UpdateCaster();
            direction = spellCaster.ragdollHand.transform.InverseTransformVector(spellCaster.ragdollHand.Velocity());
            // UP
            if (meshVisual == null)
            {
                if (spellCaster.ragdollHand.side == Side.Left)
                {
                    if (isCasting && direction.YBigger() && direction.y < -strenghtOfMvt)
                    {
                        meshVisual = new GameObject("NavMeshVisualizer").AddComponent<NavMeshVisual>();
                        meshVisual.hideNavMesh = false;
                        EndCast();
                    }
                }
                else
                {
                    if (isCasting && direction.YBigger() && direction.y > strenghtOfMvt)
                    {
                        meshVisual = new GameObject("NavMeshVisualizer").AddComponent<NavMeshVisual>();
                        meshVisual.hideNavMesh = false;
                        EndCast();
                    }
                }
            }
            // DOWN
            if (meshVisual != null)
            {
                if (spellCaster.ragdollHand.side == Side.Left)
                {
                    if (isCasting && direction.YBigger() && direction.y > strenghtOfMvt)
                    {
                        if (meshVisual.gameObject != null)
                            UnityEngine.GameObject.Destroy(meshVisual.gameObject);
                        EndCast();
                    }
                }
                else
                {
                    if (isCasting && direction.YBigger() && direction.y < -strenghtOfMvt)
                    {
                        if (meshVisual.gameObject != null)
                            UnityEngine.GameObject.Destroy(meshVisual.gameObject);
                        EndCast();
                    }
                }
            }
            if (isCasting && PlayerControl.GetHand(spellCaster.ragdollHand.side).gripPressed)
            {
                if (meshVisual != null)
                    meshVisual.increaseIntensity ^= true;
                EndCast();
            }
        }

        private void EndCast()
        {
            isCasting = false;
            spellCaster.intensity = 0.0f;
            spellCaster.Fire(isCasting);
        }
    }
}
