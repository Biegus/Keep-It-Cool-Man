using System;
using Cyberultimate.Unity;
using UnityEngine;
namespace LetterBattle
{
    public class PlayerMover : MonoBehaviour
    {
      
        [SerializeField][Range(0.1f,50)] private float speed;
        [SerializeField][Range(1f,2f)] private float velocityDegradation = 1.2f;
        [SerializeField] private SpriteRenderer spaceshipSprite;
        [SerializeField][Range(1,100)] private float rotationInertia = 5;
        
        private Vector2 previousPos;
        private Rigidbody2D rb2D;
        private float previousAngle;

        private bool anyMove = false;
        private void Awake()
        {
            rb2D = this.GetComponent<Rigidbody2D>();
            LEvents.Base.OnLevelWon.Raw += OnLevelWon;
        }

        private void OnDestroy()
        {
            LEvents.Base.OnLevelWon.Raw -= OnLevelWon;

        }

        private void OnLevelWon(object sender, EventArgs e)
        {
            if(!anyMove)
                SteamHelper.Unlock("OTHER_ACHIEVEMENT_2");
        }

        private void FixedUpdate()
        {
            this.rb2D.velocity /= velocityDegradation;
            for (int i = 0; i < 2; i++)
            {
                Vector2 pos = this.rb2D.position;
                if (this.rb2D.position[i] > CameraHelper.Current.CameraSize[i]/1.8f)
                {

                    pos[i] = previousPos[i];
                    this.rb2D.position = pos;
                }
                if (this.rb2D.position[i] < -CameraHelper.Current.CameraSize[i]/1.8f)
                {

                    pos[i] = previousPos[i];
                    this.rb2D.position = pos;
                }
            }

            previousPos = this.rb2D.position;

            
            Vector2 vel = this.rb2D.velocity;
            float angle = (float)Mathf.Atan2(vel.y, vel.x);

            Quaternion rot = Quaternion.Lerp(
                Quaternion.Euler(0, 0, previousAngle),
                Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg),
                1 / rotationInertia);
            
            spaceshipSprite.transform.rotation = rot;
            previousAngle = spaceshipSprite.transform.rotation.eulerAngles.z;
        }
        public void Push(Direction dir)
        {
            anyMove = true;
            //Debug.Log(Time.timeScale);
            this.rb2D.velocity += dir * speed;
            
        }
    }
}