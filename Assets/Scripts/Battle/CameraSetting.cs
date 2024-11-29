using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.MiniBattle.Game
{
    public class CameraSetting : MonoBehaviour
    {
        #region Public Fields

        public GameObject character;
        public GameObject map;

        public bool is_Set;

        #endregion

        #region Private Fields

        Camera cam;
        Bounds cam_Bounds;
        

        #endregion

        #region MonoBehaviour Methods

        private void Start()
        {
            cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            is_Set = false;
            

        }

        private void LateUpdate()
        {
            CamFollow();
        }

        #endregion

        #region Private Methods

        private void CamFollow()
        {
            if(character != null && map != null && is_Set)
            {               
                cam.transform.position = new Vector3(character.transform.position.x, character.transform.position.y, cam.transform.position.z);
                cam.transform.position = LimitCam();
            }
        }

        private Vector3 LimitCam()
        {
            float clampX = Mathf.Clamp(cam.transform.position.x, cam_Bounds.min.x, cam_Bounds.max.x);
            float clampY = Mathf.Clamp(cam.transform.position.y, cam_Bounds.min.y, cam_Bounds.max.y);

            return new Vector3(clampX, clampY, cam.transform.position.z);
        }

        

        #endregion

        #region Public Methods

        public void MapSetBound()
        {
            if(map != null && !is_Set )
            {
                var height = cam.orthographicSize;
                var width = height * cam.aspect;

                var bounds = map.GetComponent<BoxCollider2D>().bounds;
                Global.WorldBounds = bounds;

                var minX = Global.WorldBounds.min.x + width;
                var maxX = Global.WorldBounds.max.x - width;

                var minY = Global.WorldBounds.min.y + height;
                var maxY = Global.WorldBounds.max.y - height;

                cam_Bounds = new Bounds();
                cam_Bounds.SetMinMax(new Vector3(minX, minY, 0.0f), new Vector3(maxX, maxY, 0.0f));

                
                is_Set = true;
            }
        }

        #endregion



    }
}
