using UnityEngine;
using System.Collections;
 
public class AnimateShield : MonoBehaviour
{
    //Color of the shield (affects only illumination. Not sprite color)
    public Color shieldColor = Color.white;
    //Light prefab
    public GameObject collisionLight;
 
    //For collisions with other 2D objects
//    void OnCollisionEnter2D (Collision2D collision)
//    {
//        //We want to illuminate all contact points
//        for (int i = 0; i < collision.contacts.Length; i++) {
//            //For aesthetic purposes we want high-velocity collisions to be brighter than low-velocity collisions. "LightVelocity" determines how many lights we spawn on collision.
//            float LightVelocity = 1;
//            if (collision.relativeVelocity.magnitude >= 100) {
//                LightVelocity = 5;
//            } else if (collision.relativeVelocity.magnitude >= 75) {
//                LightVelocity = 3;
//            } else if (collision.relativeVelocity.magnitude >= 40) {
//                LightVelocity = 2;
//            } else if (collision.relativeVelocity.magnitude >= 15) {
//                LightVelocity = 1;
//            } else {
//                LightVelocity = 0;
//            }
//            //Now the actual spawning
//            for (int j = 0; j < LightVelocity; j++) {
//                //We'll spawn collisionLight prefab, at the point of collision, plus a little bit farther from the shield.
//                Light light = (Instantiate (collisionLight, collision.contacts [i].point + collision.contacts [i].normal, Quaternion.identity)as GameObject).GetComponent<Light> ();
//                light.color = shieldColor;
//                light.type = LightType.Spot;
//                light.bounceIntensity = 0;
//                //values Intensity, Range and Angle depend on your shield size. Test in editor. Make sure the range doesn't go too far behind the shield, as it will illuminate other objects.
//                light.intensity = 10;
//                light.range = 5;
//                light.spotAngle = 75;
//                light.transform.SetParent (transform);
//                //Since the sprite behaves like a sphere, thanks to the normal map, we want to illuminate it at some angle. That's why we have to edit Z position of the light.
//                light.transform.position = new Vector3 (light.transform.position.x, light.transform.position.y, -1);
//                light.transform.LookAt (collision.contacts [i].point);
//            }
// 
//        }
//    }
 
    //For manual activation through script (good for weapon hits). Same as above, but only spawns one light.
    public void OnHit (RaycastHit2D hit)
    {
        Light light = (Instantiate (collisionLight, hit.point + (hit.normal * 0.5f), Quaternion.identity)as GameObject).GetComponent<Light> ();
        light.color = shieldColor;
        light.type = LightType.Spot;
        light.bounceIntensity = 0;
        light.intensity = 10;
        light.range = 5;
        light.spotAngle = 75;
        light.transform.SetParent (transform);
        light.transform.LookAt (hit.point);
 
    }
 
//    //For collisions with particles
//    void OnParticleCollision (GameObject psystem)
//    {
//        //As noted in the unity documentation, the parameter links to the particle system
//        ParticleSystem particleSystem = psystem.GetComponent<ParticleSystem> ();
//        ParticleCollisionEvent[] collisions = new ParticleCollisionEvent[particleSystem.GetSafeCollisionEventSize ()];
//        //This returns count of collisions between "gameObject" and the particle system and puts all those collisions to "collisions"
//        int max = particleSystem.GetCollisionEvents (gameObject, collisions);
//        //The rest is same as above.
//        for (int i = 0; i < max; i++) {
//            float LightVelocityPos = 1;
//            if (collisions [i].velocity.magnitude >= 100) {
//                LightVelocityPos = 5;
//            } else if (collisions [i].velocity.magnitude >= 75) {
//                LightVelocityPos = 3;
//            } else if (collisions [i].velocity.magnitude >= 40) {
//                LightVelocityPos = 2;
//            } else if (collisions [i].velocity.magnitude >= 15) {
//                LightVelocityPos = 1;
//            } else {
//                LightVelocityPos = 0;
//            }
//            for (int j = 0; j < LightVelocityPos + 1; j++) {
//                Light light = (Instantiate (collisionLight, collisions [i].intersection + collisions [i].normal, Quaternion.LookRotation (collisions [i].normal))as GameObject).GetComponent<Light> ();
//                light.color = shieldColor;
//                light.type = LightType.Spot;
//                light.bounceIntensity = 0;
//                light.intensity = 10;
//                light.range = 5;
//                light.spotAngle = 75;
//                light.transform.SetParent (transform);
//                light.transform.position = new Vector3 (light.transform.position.x, light.transform.position.y, -1);
//                light.transform.LookAt (collisions [i].intersection);
//            }
//        }
//    }
}