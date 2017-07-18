namespace Assets.Scripts.UI
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [ExecuteInEditMode]
    public class CUI3DImageScript : CUIComponent
    {
        private ListView<C3DGameObject> m_3DGameObjects = new ListView<C3DGameObject>();
        public en3DImageLayer m_imageLayer;
        private Vector2 m_lastPivotScreenPosition;
        private Vector2 m_pivotScreenPosition;
        public Camera m_renderCamera;
        public Vector3 m_renderCameraDefaultScale = Vector3.get_one();
        public float m_renderCameraDefaultSize = 20f;
        private Light m_renderLight;
        public static int[] s_cameraDepths = new int[] { 9, 11 };
        public static int[] s_cameraLayers = new int[] { 0x10, 0x11 };

        public GameObject AddGameObject(string path, bool useGameObjectPool, bool needCached = false)
        {
            return this.AddGameObject(path, useGameObjectPool, ref this.m_pivotScreenPosition, true, needCached, null);
        }

        public GameObject AddGameObject(string path, bool useGameObjectPool, ref Vector2 screenPosition, bool needCached = false)
        {
            return this.AddGameObject(path, useGameObjectPool, ref screenPosition, false, needCached, null);
        }

        public GameObject AddGameObject(string path, bool useGameObjectPool, ref Vector2 screenPosition, bool bindPivot, bool needCached = false, string pathToAdd = new string())
        {
            GameObject gameObject = null;
            if (useGameObjectPool)
            {
                gameObject = Singleton<CGameObjectPool>.GetInstance().GetGameObject(path, enResourceType.UI3DImage);
            }
            else
            {
                GameObject content = Singleton<CResourceManager>.GetInstance().GetResource(path, typeof(GameObject), enResourceType.UI3DImage, needCached, false).m_content;
                if (content != null)
                {
                    gameObject = Object.Instantiate(content);
                }
            }
            if (gameObject == null)
            {
                return null;
            }
            Vector3 vector = gameObject.get_transform().get_localScale();
            if (pathToAdd == null)
            {
                gameObject.get_transform().SetParent(base.get_gameObject().get_transform(), true);
            }
            else
            {
                Transform transform = base.get_gameObject().get_transform().Find(pathToAdd);
                if (transform != null)
                {
                    gameObject.get_transform().SetParent(transform, true);
                }
            }
            gameObject.get_transform().set_localPosition(Vector3.get_zero());
            gameObject.get_transform().set_localRotation(Quaternion.get_identity());
            C3DGameObject item = new C3DGameObject();
            item.m_gameObject = gameObject;
            item.m_path = path;
            item.m_useGameObjectPool = useGameObjectPool;
            item.m_protogenic = false;
            item.m_bindPivot = bindPivot;
            this.m_3DGameObjects.Add(item);
            if (this.m_renderCamera.get_orthographic())
            {
                this.ChangeScreenPositionToWorld(gameObject, ref screenPosition);
                if (!this.m_renderCamera.get_enabled() && (this.m_3DGameObjects.Count > 0))
                {
                    this.m_renderCamera.set_enabled(true);
                }
            }
            else
            {
                Transform transform2 = base.get_transform().FindChild("_root");
                if (transform2 != null)
                {
                    if (pathToAdd == null)
                    {
                        gameObject.get_transform().SetParent(transform2, true);
                    }
                    else
                    {
                        Transform transform3 = base.get_gameObject().get_transform().Find(pathToAdd);
                        if (transform3 != null)
                        {
                            gameObject.get_transform().SetParent(transform3, true);
                        }
                    }
                    gameObject.get_transform().set_localPosition(Vector3.get_zero());
                    gameObject.get_transform().set_localRotation(Quaternion.get_identity());
                    gameObject.get_transform().set_localScale(vector);
                }
            }
            CUIUtility.SetGameObjectLayer(gameObject, !this.m_renderCamera.get_enabled() ? 0x1f : s_cameraLayers[(int) this.m_imageLayer]);
            return gameObject;
        }

        public GameObject AddGameObjectToPath(string path, bool useGameObjectPool, string pathToAdd)
        {
            return this.AddGameObject(path, useGameObjectPool, ref this.m_pivotScreenPosition, false, false, pathToAdd);
        }

        public override void Appear()
        {
            base.Appear();
            if (this.m_renderCamera != null)
            {
                this.m_renderCamera.set_enabled(true);
            }
            for (int i = 0; i < this.m_3DGameObjects.Count; i++)
            {
                CUIUtility.SetGameObjectLayer(this.m_3DGameObjects[i].m_gameObject, s_cameraLayers[(int) this.m_imageLayer]);
            }
        }

        public void ChangeScreenPositionToWorld(string path, ref Vector2 screenPosition)
        {
            this.ChangeScreenPositionToWorld(this.GetGameObject(path), ref screenPosition);
        }

        public void ChangeScreenPositionToWorld(GameObject gameObject, ref Vector2 screenPosition)
        {
            if (gameObject != null)
            {
                gameObject.get_transform().set_position(CUIUtility.ScreenToWorldPoint(this.m_renderCamera, screenPosition, 100f));
            }
        }

        public override void Close()
        {
            base.Close();
            int index = 0;
            while (index < this.m_3DGameObjects.Count)
            {
                if (!this.m_3DGameObjects[index].m_protogenic)
                {
                    if (this.m_3DGameObjects[index].m_gameObject != null)
                    {
                        if (this.m_3DGameObjects[index].m_useGameObjectPool)
                        {
                            Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.m_3DGameObjects[index].m_gameObject);
                        }
                        else
                        {
                            Object.Destroy(this.m_3DGameObjects[index].m_gameObject);
                        }
                    }
                    this.m_3DGameObjects[index].m_path = null;
                    this.m_3DGameObjects[index].m_gameObject = null;
                    this.m_3DGameObjects.RemoveAt(index);
                }
                else
                {
                    index++;
                }
            }
        }

        public GameObject GetGameObject(string path)
        {
            for (int i = 0; i < this.m_3DGameObjects.Count; i++)
            {
                if (this.m_3DGameObjects[i].m_path.Equals(path))
                {
                    return this.m_3DGameObjects[i].m_gameObject;
                }
            }
            return null;
        }

        public Vector2 GetPivotScreenPosition()
        {
            this.m_pivotScreenPosition = CUIUtility.WorldToScreenPoint(base.m_belongedFormScript.GetCamera(), base.get_gameObject().get_transform().get_position());
            return this.m_pivotScreenPosition;
        }

        public override void Hide()
        {
            base.Hide();
            if (this.m_renderCamera != null)
            {
                this.m_renderCamera.set_enabled(false);
            }
            for (int i = 0; i < this.m_3DGameObjects.Count; i++)
            {
                CUIUtility.SetGameObjectLayer(this.m_3DGameObjects[i].m_gameObject, 0x1f);
            }
        }

        public override void Initialize(CUIFormScript formScript)
        {
            if (!base.m_isInitialized)
            {
                base.Initialize(formScript);
                this.m_renderCamera = base.get_gameObject().GetComponent<Camera>();
                this.m_renderLight = base.get_gameObject().GetComponent<Light>();
                this.InitializeRender();
                this.GetPivotScreenPosition();
                this.Initialize3DGameObjects();
            }
        }

        private void Initialize3DGameObjects()
        {
            this.m_3DGameObjects.Clear();
            for (int i = 0; i < base.get_gameObject().get_transform().get_childCount(); i++)
            {
                GameObject gameObject = base.get_gameObject().get_transform().GetChild(i).get_gameObject();
                CUIUtility.SetGameObjectLayer(gameObject, s_cameraLayers[(int) this.m_imageLayer]);
                if (this.m_renderCamera.get_orthographic())
                {
                    this.ChangeScreenPositionToWorld(gameObject, ref this.m_pivotScreenPosition);
                }
                C3DGameObject item = new C3DGameObject();
                item.m_path = gameObject.get_name();
                item.m_gameObject = gameObject;
                item.m_useGameObjectPool = false;
                item.m_protogenic = true;
                item.m_bindPivot = true;
                this.m_3DGameObjects.Add(item);
            }
            this.m_renderCamera.set_enabled(this.m_3DGameObjects.Count > 0);
        }

        public void InitializeRender()
        {
            if (this.m_renderCamera != null)
            {
                this.m_renderCamera.set_clearFlags(3);
                this.m_renderCamera.set_cullingMask(((int) 1) << s_cameraLayers[(int) this.m_imageLayer]);
                this.m_renderCamera.set_depth((float) s_cameraDepths[(int) this.m_imageLayer]);
                if (this.m_renderCamera.get_orthographic())
                {
                    this.m_renderCamera.set_orthographicSize(this.m_renderCameraDefaultSize * ((base.m_belongedFormScript.get_transform() as RectTransform).get_rect().get_height() / base.m_belongedFormScript.GetReferenceResolution().y));
                }
                else
                {
                    this.m_renderCamera.get_gameObject().get_transform().set_localScale((Vector3) (this.m_renderCameraDefaultScale * (1f / ((base.m_belongedFormScript.get_gameObject().get_transform().get_localScale().x != 0f) ? base.m_belongedFormScript.get_gameObject().get_transform().get_localScale().x : 1f))));
                }
            }
            if (this.m_renderLight != null)
            {
                this.m_renderLight.set_cullingMask(((int) 1) << s_cameraLayers[(int) this.m_imageLayer]);
            }
        }

        protected override void OnDestroy()
        {
            this.m_renderCamera = null;
            this.m_renderLight = null;
            this.m_3DGameObjects.Clear();
            this.m_3DGameObjects = null;
            base.OnDestroy();
        }

        public void RemoveGameObject(string path)
        {
            int index = 0;
            while (index < this.m_3DGameObjects.Count)
            {
                if (string.Equals(this.m_3DGameObjects[index].m_path, path, StringComparison.OrdinalIgnoreCase))
                {
                    if (this.m_3DGameObjects[index].m_useGameObjectPool)
                    {
                        Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.m_3DGameObjects[index].m_gameObject);
                    }
                    else
                    {
                        Object.Destroy(this.m_3DGameObjects[index].m_gameObject);
                    }
                    this.m_3DGameObjects.RemoveAt(index);
                }
                else
                {
                    index++;
                }
            }
            if (this.m_3DGameObjects.Count <= 0)
            {
                this.m_renderCamera.set_enabled(false);
            }
        }

        public void RemoveGameObject(GameObject removeObj)
        {
            if (removeObj != null)
            {
                int index = 0;
                while (index < this.m_3DGameObjects.Count)
                {
                    if (this.m_3DGameObjects[index].m_gameObject == removeObj)
                    {
                        if (this.m_3DGameObjects[index].m_useGameObjectPool)
                        {
                            Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.m_3DGameObjects[index].m_gameObject);
                        }
                        else
                        {
                            Object.Destroy(this.m_3DGameObjects[index].m_gameObject);
                        }
                        this.m_3DGameObjects.RemoveAt(index);
                    }
                    else
                    {
                        index++;
                    }
                }
                if (this.m_3DGameObjects.Count <= 0)
                {
                    this.m_renderCamera.set_enabled(false);
                }
            }
        }

        private void Update()
        {
            if ((base.m_belongedFormScript == null) || !base.m_belongedFormScript.IsClosed())
            {
                this.GetPivotScreenPosition();
                if (this.m_lastPivotScreenPosition != this.m_pivotScreenPosition)
                {
                    if (this.m_renderCamera != null)
                    {
                        if (this.m_renderCamera.get_orthographic())
                        {
                            for (int i = 0; i < this.m_3DGameObjects.Count; i++)
                            {
                                if (this.m_3DGameObjects[i].m_bindPivot)
                                {
                                    this.ChangeScreenPositionToWorld(this.m_3DGameObjects[i].m_gameObject, ref this.m_pivotScreenPosition);
                                }
                            }
                        }
                        else
                        {
                            float offsetX = this.m_pivotScreenPosition.x / ((float) Mathf.Max(Screen.get_width(), Screen.get_height()));
                            offsetX = (offsetX * 2f) - 1f;
                            this.m_renderCamera.set_rect(new Rect(0f, 0f, 1f, 1f));
                            this.m_renderCamera.ResetAspect();
                            this.m_renderCamera.SetOffsetX(offsetX);
                        }
                    }
                    this.m_lastPivotScreenPosition = this.m_pivotScreenPosition;
                }
            }
        }

        private class C3DGameObject
        {
            public bool m_bindPivot;
            public GameObject m_gameObject;
            public string m_path;
            public bool m_protogenic;
            public bool m_useGameObjectPool;
        }
    }
}

