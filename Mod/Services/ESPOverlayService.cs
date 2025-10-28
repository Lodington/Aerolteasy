using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using RoR2;
using RoR2DevTool.Models;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RoR2DevTool.Services
{
    public class ESPOverlayService
    {
        private ManualLogSource logger;
        private GameStateService gameStateService;
        private Canvas overlayCanvas;
        private Dictionary<int, GameObject> activeLabels = new Dictionary<int, GameObject>();
        
        // ESP Settings
        public bool ShowMonsterLabels { get; set; } = false;
        public bool ShowInteractableLabels { get; set; } = false;
        public bool ShowItemLabels { get; set; } = false;
        public bool ShowDistances { get; set; } = true;
        public bool ShowHealth { get; set; } = true;
        public float MaxLabelDistance { get; set; } = 100f;
        public float LabelScale { get; set; } = 1.0f;

        public ESPOverlayService(ManualLogSource logger, GameStateService gameStateService)
        {
            this.logger = logger;
            this.gameStateService = gameStateService;
        }

        public void Initialize()
        {
            try
            {
                CreateOverlayCanvas();
                logger.LogInfo("ESP Overlay Service initialized");
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to initialize ESP Overlay Service: {ex.Message}");
            }
        }

        private void CreateOverlayCanvas()
        {
            try
            {
                // Create a canvas for ESP overlays
                var canvasObject = new GameObject("ESPOverlayCanvas");
                GameObject.DontDestroyOnLoad(canvasObject);
                
                overlayCanvas = canvasObject.AddComponent<Canvas>();
                overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                overlayCanvas.sortingOrder = 100; // Render on top of other UI
                
                var canvasScaler = canvasObject.AddComponent<CanvasScaler>();
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.referenceResolution = new Vector2(1920, 1080);
                canvasScaler.matchWidthOrHeight = 0.5f;
                
                canvasObject.AddComponent<GraphicRaycaster>();
                
                // Set canvas size to match screen
                var rectTransform = canvasObject.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
                
                logger.LogInfo($"ESP overlay canvas created successfully (Screen: {Screen.width}x{Screen.height})");
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to create overlay canvas: {ex.Message}");
            }
        }

        public void UpdateOverlays()
        {
            try
            {
                if (overlayCanvas == null || Run.instance == null)
                {
                    return;
                }

                // Clear existing labels
                ClearLabels();

                var gameState = gameStateService?.GetGameState();
                if (gameState == null)
                {
                    return;
                }

                var playerBody = LocalUserManager.GetFirstLocalUser()?.cachedBody;
                if (playerBody == null) return;

                var playerPosition = playerBody.transform.position;
                var camera = Camera.main;
                
                if (camera == null) return;

                // Create monster labels
                if (ShowMonsterLabels && gameState.Monsters != null)
                {
                    foreach (var monster in gameState.Monsters)
                    {
                        if (monster != null && monster.Distance <= MaxLabelDistance)
                        {
                            CreateMonsterLabel(monster, camera, playerPosition);
                        }
                    }
                }

                // Create interactable labels
                if (ShowInteractableLabels && gameState.Interactables != null)
                {
                    foreach (var interactable in gameState.Interactables)
                    {
                        if (interactable != null && interactable.Distance <= MaxLabelDistance)
                        {
                            CreateInteractableLabel(interactable, camera, playerPosition);
                        }
                    }
                }

                // Create item labels
                if (ShowItemLabels && gameState.Items != null)
                {
                    foreach (var item in gameState.Items)
                    {
                        if (item != null && item.Distance <= MaxLabelDistance)
                        {
                            CreateItemLabel(item, camera, playerPosition);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning($"Error updating ESP overlays: {ex.Message}");
                if (ex.StackTrace != null)
                {
                    logger.LogWarning($"Stack trace: {ex.StackTrace}");
                }
            }
        }

        private void CreateMonsterLabel(MonsterData monster, Camera camera, Vector3 playerPosition)
        {
            var worldPos = new Vector3(monster.PositionX, monster.PositionY + 2f, monster.PositionZ);
            var screenPos = camera.WorldToScreenPoint(worldPos);
            
            // Check if behind camera
            if (screenPos.z <= 0) return;
            
            var labelObject = CreateLabelObject($"Monster_{monster.NetworkId}");
            if (labelObject == null) return;
            
            var text = labelObject.GetComponent<Text>();
            if (text == null) return;
            
            // Build label text
            var labelText = monster.DisplayName;
            
            if (ShowHealth && monster.MaxHealth > 0)
            {
                var healthPercent = (monster.Health / monster.MaxHealth) * 100f;
                labelText += $"\n❤️ {healthPercent:F0}%";
            }
            
            if (ShowDistances)
            {
                labelText += $"\n📍 {monster.Distance:F1}m";
            }
            
            if (monster.IsElite)
            {
                labelText += $"\n👑 {monster.EliteType}";
            }
            
            text.text = labelText;
            text.color = monster.IsElite ? new Color(1f, 0.4f, 1f) : new Color(1f, 0.3f, 0.3f); // Bright magenta for elites, bright red for normal
            
            PositionLabel(labelObject, screenPos);
        }

        private void CreateInteractableLabel(InteractableData interactable, Camera camera, Vector3 playerPosition)
        {
            var worldPos = new Vector3(interactable.PositionX, interactable.PositionY + 1f, interactable.PositionZ);
            var screenPos = camera.WorldToScreenPoint(worldPos);
            
            if (screenPos.z <= 0) return;
            
            var labelObject = CreateLabelObject($"Interactable_{interactable.NetworkId}");
            if (labelObject == null) return;
            
            var text = labelObject.GetComponent<Text>();
            if (text == null) return;
            
            var labelText = interactable.DisplayName;
            
            if (ShowDistances)
            {
                labelText += $"\n📍 {interactable.Distance:F1}m";
            }
            
            labelText += $"\n📦 {interactable.Category}";
            
            text.text = labelText;
            text.color = interactable.IsAvailable ? new Color(0.2f, 0.8f, 0.2f) : new Color(0.6f, 0.6f, 0.6f); // Bright green for available, medium gray for unavailable
            
            PositionLabel(labelObject, screenPos);
        }

        private void CreateItemLabel(ItemData item, Camera camera, Vector3 playerPosition)
        {
            var worldPos = new Vector3(item.PositionX, item.PositionY + 0.5f, item.PositionZ);
            var screenPos = camera.WorldToScreenPoint(worldPos);
            
            if (screenPos.z <= 0) return;
            
            var labelObject = CreateLabelObject($"Item_{item.NetworkId}");
            var text = labelObject.GetComponent<TextMeshProUGUI>();
            
            var labelText = item.DisplayName;
            
            if (ShowDistances)
            {
                labelText += $"\n📍 {item.Distance:F1}m";
            }
            
            labelText += $"\n💎 {item.ItemTier}";
            
            text.text = labelText;
            text.color = GetItemTierColor(item.ItemTier);
            
            PositionLabel(labelObject, screenPos);
        }

        private GameObject CreateLabelObject(string id)
        {
            try
            {
                if (overlayCanvas == null)
                {
                    logger.LogWarning("Cannot create label: overlay canvas is null");
                    return null;
                }

                var labelObject = new GameObject($"ESPLabel_{id}");
                labelObject.transform.SetParent(overlayCanvas.transform, false);
                
                var text = labelObject.AddComponent<Text>();
                
                // Use Unity's built-in font which is more reliable
                try
                {
                    var font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                    if (font != null)
                    {
                        text.font = font;
                    }
                    else
                    {
                        // Fallback to default font
                        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.fontsettings");
                    }
                }
                catch (Exception fontEx)
                {
                    logger.LogWarning($"Could not load font: {fontEx.Message}");
                    // Unity Text will use default font if none is set
                }
                
                text.fontSize = Mathf.RoundToInt(14f * LabelScale);
                text.alignment = TextAnchor.MiddleCenter;
                text.horizontalOverflow = HorizontalWrapMode.Overflow;
                text.verticalOverflow = VerticalWrapMode.Overflow;
                
                // Add outline component for better visibility
                try
                {
                    var outline = labelObject.AddComponent<Outline>();
                    outline.effectColor = Color.black;
                    outline.effectDistance = new Vector2(1, 1);
                }
                catch (Exception outlineEx)
                {
                    logger.LogWarning($"Could not add text outline: {outlineEx.Message}");
                }
                
                var rectTransform = labelObject.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.sizeDelta = new Vector2(200, 100);
                }
                
                var hashKey = id.GetHashCode();
                
                // If a label with this ID already exists, destroy it first
                if (activeLabels.ContainsKey(hashKey))
                {
                    var existingLabel = activeLabels[hashKey];
                    if (existingLabel != null)
                    {
                        existingLabel.SetActive(false);
                        GameObject.DestroyImmediate(existingLabel);
                    }
                }
                
                activeLabels[hashKey] = labelObject;
                
                return labelObject;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error creating label object: {ex.Message}");
                return null;
            }
        }

        private void PositionLabel(GameObject labelObject, Vector3 screenPos)
        {
            try
            {
                var rectTransform = labelObject.GetComponent<RectTransform>();
                if (rectTransform == null) return;
                
                // For screen space overlay, convert screen position to anchored position
                // Screen position is already in screen coordinates, we just need to adjust for canvas
                var canvasRect = overlayCanvas.GetComponent<RectTransform>();
                if (canvasRect == null) return;
                
                // Convert screen position to canvas anchored position
                // Adjust for canvas size and screen size differences
                var canvasSize = canvasRect.sizeDelta;
                var screenSize = new Vector2(Screen.width, Screen.height);
                
                // Calculate position relative to canvas
                var normalizedPos = new Vector2(
                    screenPos.x / screenSize.x,
                    screenPos.y / screenSize.y
                );
                
                var canvasPos = new Vector2(
                    (normalizedPos.x - 0.5f) * canvasSize.x,
                    (normalizedPos.y - 0.5f) * canvasSize.y
                );
                
                rectTransform.anchoredPosition = canvasPos;
            }
            catch (Exception ex)
            {
                logger.LogWarning($"Error positioning label: {ex.Message}");
            }
        }

        private Color GetItemTierColor(string tier)
        {
            return tier switch
            {
                "White" => new Color(0.9f, 0.9f, 0.9f), // Light gray instead of pure white for better readability
                "Green" => new Color(0.2f, 0.8f, 0.2f), // Bright green
                "Red" => new Color(1f, 0.3f, 0.3f), // Bright red
                "Lunar" => new Color(0.4f, 0.8f, 1f), // Bright cyan
                "Boss" => new Color(1f, 0.8f, 0.2f), // Bright yellow/orange
                "Void White" => new Color(0.7f, 0.5f, 1f), // Light purple
                "Void Green" => new Color(0.6f, 0.4f, 0.9f), // Medium purple
                "Void Red" => new Color(0.8f, 0.3f, 1f), // Bright purple
                "Void Boss" => new Color(0.9f, 0.4f, 1f), // Very bright purple
                _ => new Color(0.8f, 0.8f, 0.8f) // Light gray for unknown
            };
        }

        private void ClearLabels()
        {
            foreach (var label in activeLabels.Values)
            {
                if (label != null)
                {
                    // Immediately deactivate to prevent duplicates
                    label.SetActive(false);
                    GameObject.DestroyImmediate(label);
                }
            }
            activeLabels.Clear();
        }

        public void SetMonsterLabels(bool enabled)
        {
            ShowMonsterLabels = enabled;
            logger.LogInfo($"Monster labels {(enabled ? "enabled" : "disabled")}");
        }

        public void SetInteractableLabels(bool enabled)
        {
            ShowInteractableLabels = enabled;
            logger.LogInfo($"Interactable labels {(enabled ? "enabled" : "disabled")}");
        }

        public void SetItemLabels(bool enabled)
        {
            ShowItemLabels = enabled;
            logger.LogInfo($"Item labels {(enabled ? "enabled" : "disabled")}");
        }

        public void SetLabelDistance(float distance)
        {
            MaxLabelDistance = distance;
            logger.LogInfo($"Label distance set to {distance}m");
        }

        public void SetLabelScale(float scale)
        {
            LabelScale = Mathf.Clamp(scale, 0.5f, 3.0f);
            logger.LogInfo($"Label scale set to {LabelScale}");
        }

        public void DisableAllOverlays()
        {
            ShowMonsterLabels = false;
            ShowInteractableLabels = false;
            ShowItemLabels = false;
            ClearLabels();
            logger.LogInfo("All ESP overlays disabled");
        }

        public void Cleanup()
        {
            try
            {
                ClearLabels();
                if (overlayCanvas != null)
                {
                    GameObject.Destroy(overlayCanvas.gameObject);
                }
                logger.LogInfo("ESP Overlay Service cleaned up");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error cleaning up ESP Overlay Service: {ex.Message}");
            }
        }
    }
}