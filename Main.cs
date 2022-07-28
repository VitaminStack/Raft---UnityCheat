
using Steamworks;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.AzureSky;

namespace UnityHack
{


    public class Loader
    {
        private static GameObject go;


        public static void Init()
        {
            go = new GameObject();
            go.AddComponent<Main>();
            UnityEngine.Object.DontDestroyOnLoad(go);
        }

        public static void Unload()
        {
            UnityEngine.Object.Destroy(go);
        }
    }


    public class Hax
    {
        static public bool InventoryCheck = true;
        static public bool Cheat1 = false;
        static public bool Cheat2 = false;
        static public bool Cheat3 = false;
        static public bool Cheat4 = false;

        static public bool ESP = true;
        static public bool ItemESP = true;
        static public bool Testing = false;

        
        static public Color AIColor = new Color(1f, 0.5f, 0.1f, 1);
       
        public static Camera _Camera;
       
        public static List<RecipeMenuItem> GetPrivate() //zugriff auf Private 
        {
            var CMenu = ComponentManager<CraftingMenu>.Value;
            return (List<RecipeMenuItem>)typeof(CraftingMenu).GetField("recipeMenuItems").GetValue(CMenu);            
        }
        


        public static void RenderMenu()
        {
            
            Render.DrawBoxOutline(new Vector2(100, 50), 50, 25, Color.red, 2);
            GUI.Label(new Rect(100, 50, 50, 50), "Injected");
            GUI.color = Color.green;
            Render.DrawString(new Vector2(100, 80), "NUMPAD1 -> NoClip", false);
            Render.DrawString(new Vector2(100, 100), "NUMPAD2 -> Item ESP (Gelb)", false);
            Render.DrawString(new Vector2(100, 120), "NUMPAD3 -> Player/Animal ESP (Rot)", false);
            Render.DrawString(new Vector2(100, 140), "NUMPAD4 -> Bout BOOOST", false);
            Render.DrawString(new Vector2(100, 160), "NUMPAD5 -> Debug Godmode", false);
            Render.DrawString(new Vector2(340, 80), "NUMPAD6 -> SonnenEditor Pfeiltasten", false);
            Render.DrawString(new Vector2(340, 100), "NUMPAD8 -> item Sauger", false);
            Render.DrawString(new Vector2(340, 120), "NUMPAD9 -> Teleport to other random Player", false);
        }
        public static void RenderESP()
        {
            
            foreach (var EntP in NetworkIDManager.GetNetworkdIDs<Network_Entity>())
            {
               
                if (EntP is Network_Entity && ESP)
                {
                    float Dist = Vector3.Distance(Hax._Camera.transform.position, EntP.transform.position);
                    Dist = (int)(Dist * 100) / 100;
                    
                    Vector3 w2s_TEST1 = Hax._Camera.WorldToScreenPoint(EntP.transform.position);
                    Vector3 PPos = EntP.transform.position;
                    PPos.y -= 0.8f;
                    Vector3 Pw2S = Hax._Camera.WorldToScreenPoint(PPos);
                                       
                    if (EntP.name.StartsWith("AI") || EntP.name.StartsWith("Sea"))
                    {
                        if (w2s_TEST1.z > 0f && Dist <= 400)
                        {
                            string Playername = EntP.name.Replace("PlayerStats, ", "");
                            Playername = Playername.Replace("(Clone)", "");
                            Playername = Playername.Replace("Network_Player,", "");
                            
                            GUI.color = Hax.AIColor;
                            Render.DrawString(new Vector2(w2s_TEST1.x, Screen.height - w2s_TEST1.y), Playername + " " + Dist.ToString() + "m", true);
                        }
                    }

                    if (!EntP.name.StartsWith("AI") && !EntP.name.StartsWith("Sea"))
                    {                       
                        if (Pw2S.z > 0f && Dist <= 400)
                        {
                            string Playername = EntP.name.Replace("PlayerStats, ", "");
                            Playername = Playername.Replace("(Clone)", "");
                            Playername = Playername.Replace("Network_Player,", "");

                            GUI.color = Color.red;
                            Render.DrawString(new Vector2(Pw2S.x, Screen.height - Pw2S.y), Playername + " " + Dist.ToString() + "m", true);
                        }
                    }
                }
            }
            foreach (var EntI in NetworkIDManager.GetNetworkdIDs<PickupItem_Networked>())//frisst performance 200fps auf 60fps
            {
                if (EntI is PickupItem_Networked && EntI.CanBePickedUp() && ItemESP)
                {
                    float Dist = Vector3.Distance(Hax._Camera.transform.position, EntI.transform.position);
                    Dist = (int)(Dist * 100) / 100;

                    Vector3 Pos = EntI.transform.position;
                    Vector3 w2s_TEST1 = Hax._Camera.WorldToScreenPoint(Pos);


                    if (Dist < 30 && w2s_TEST1.z > 0f && EntI.PickupItem.pickupItemType != PickupItemType.QuestItem && EntI.PickupItem.pickupItemType
                        != PickupItemType.DomesticAnimal && EntI.name != ("PickupChannelingCube") && !EntI.name.StartsWith("Pickup_Flo"))
                    {
                        string name = EntI.PickupItem.PickupName;
                        GUI.color = Color.yellow;
                        Render.DrawString(new Vector2(w2s_TEST1.x, Screen.height - w2s_TEST1.y), name + " " + Dist.ToString() + "m", true);
                    }
                    if (Dist < 50 && w2s_TEST1.z > 0f && EntI.PickupItem.pickupItemType == PickupItemType.QuestItem)
                    {
                        string name = EntI.PickupItem.PickupName;
                        GUI.color = Color.magenta;
                        Render.DrawString(new Vector2(w2s_TEST1.x, Screen.height - w2s_TEST1.y), name + " " + Dist.ToString() + "m", true);
                    }
                }
            }

            
            var LPlayer = ComponentManager<Network_Player>.Value;         
            foreach(Transform Pos in LPlayer.currentModel.skeletonBone)
            {
                Vector3 w2s = Hax._Camera.WorldToScreenPoint(Pos.position);
                if (w2s.z > 0)
                {
                    GUI.color = Color.red;
                    Render.DrawString(new Vector2(w2s.x, Screen.height - w2s.y), "bone", true);
                }
            }
            

        }
    }
    public class Main : MonoBehaviour
    {
        private FieldInfo PaddleForcefield;
        private void Update()
        {            
            //Inventory Cheat
            if (Hax.InventoryCheck)
            {
                var Invs = UnityEngine.Object.FindObjectsOfType<PlayerInventory>();
                
                foreach (var Inv in Invs)
                {
                    if(Inv.transform.parent != null)
                    {
                        Inv.SetBackpackActiveSlots(30);
                        Hax.InventoryCheck = false;
                    }                                   
                }
            }
            
            Hax._Camera = Camera.main;
            if (Input.GetKey(KeyCode.Insert))
            {
                Loader.Unload();
            }
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {                
                var Players = UnityEngine.Object.FindObjectsOfType<FlightCamera>();                
                foreach (var Entity in Players)
                {
                    Entity.Toggle(true);                    
                }
            }            
            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                Hax.ItemESP = !Hax.ItemESP;
            }
            if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                Hax.ESP = !Hax.ESP;
            }
            if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                Hax.Cheat2 = !Hax.Cheat2;
            }
            if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                Hax.Cheat1 = !Hax.Cheat1;
            }
            if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                Hax.Cheat3 = !Hax.Cheat3;
            }
            if (Input.GetKeyDown(KeyCode.Keypad6)) 
            {
                Hax.Cheat4 = !Hax.Cheat4;
                //Cheat.AllowCheatsForLocalPlayer();
                //Cheat.UseCheats = true;
            }
            
            if (Hax.Cheat1)
            {                
                var LocalPlayer = ComponentManager<Network_Player>.Value;
                LocalPlayer.Stats.stat_health.SetToMaxValue();
                LocalPlayer.Stats.stat_oxygen.SetToMaxValue();
                LocalPlayer.Stats.stat_hunger.Normal.SetToMaxValue();
                LocalPlayer.Stats.stat_thirst.Normal.SetToMaxValue();
                var Item = LocalPlayer.Inventory.GetSelectedHotbarItem();
                Item.SetUsesToMax();
                
                //var Recipes = Hax.GetPrivate();       //Rezept null kosten funzt noch nicht          
                //foreach(var Rezept in Recipes)
                //{
                //    var subitems = (List<RecipeMenuSubItem>)typeof(RecipeMenuItem).GetField("subItems").GetValue(Rezept);
                //    foreach (var Rval in subitems)
                //    {
                //        foreach(var Cost in Rval.recipeItem.settings_recipe.NewCost)
                //        {                            
                //                Cost.amount = 0;                            
                //        }
                //    }
                //}



            }
            if (Hax.Cheat2)
            {
                var Boat = ComponentManager<Raft>.Value;
                var LocalP = ComponentManager<Network_Player>.Value;
                var Item = LocalP.Inventory.GetSelectedHotbarItem();
                Item.SetUsesToMax();
                if (PaddleForcefield == null)
                {
                    PaddleForcefield = typeof(Paddle).GetField("paddleForce");
                }                
                if (PaddleForcefield != null)
                {
                    PaddleForcefield.SetValue(LocalP.PaddleScript, 50f);
                }                
                Boat.maxVelocity = 50.0f;
                LocalP.PaddleScript.OnPaddle();
            }
            if (Hax.Cheat3)
            {
                var Pickables = UnityEngine.Object.FindObjectsOfType<PickupItem>();
                var LocalP = ComponentManager<Network_Player>.Value;
                foreach (var pickupItem in Pickables)
                {
                    float Dist = Vector3.Distance(Hax._Camera.transform.position, pickupItem.transform.position);
                    if (Dist < 20)
                    {
                        LocalP.PickupScript.AddItemToInventory(pickupItem);
                        LocalP.PickupScript.PickupItem(pickupItem, true, false);
                    }
                }               
            }
            if(Hax.Cheat4)
            {
                bool key = Input.GetKey(KeyCode.LeftShift);
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    AzureSkyController value = ComponentManager<AzureSkyController>.Value;
                    float num = value.timeOfDay.longitude;
                    num += (float)(key ? 2 : 1);
                    value.timeOfDay.longitude = ((num > 180f) ? -180f : num);
                }
                else if (Input.GetKey(KeyCode.LeftArrow))
                {
                    AzureSkyController value2 = ComponentManager<AzureSkyController>.Value;
                    float num2 = value2.timeOfDay.longitude;
                    num2 -= (float)(key ? 2 : 1);
                    value2.timeOfDay.longitude = ((num2 < -180f) ? 180f : num2);
                }
                else if (Input.GetKey(KeyCode.UpArrow))
                {
                    AzureSkyController value3 = ComponentManager<AzureSkyController>.Value;
                    float num3 = value3.timeOfDay.hour;
                    if (key)
                    {
                        num3 += 0.2f;
                    }
                    else
                    {
                        num3 += 0.05f;
                    }
                    if (num3 >= 24f)
                    {
                        num3 -= 24f;
                    }
                    value3.timeOfDay.GotoTime(num3);
                }
                else if (Input.GetKey(KeyCode.DownArrow))
                {
                    AzureSkyController value4 = ComponentManager<AzureSkyController>.Value;
                    float num4 = value4.timeOfDay.hour;
                    if (key)
                    {
                        num4 -= 0.2f;
                    }
                    else
                    {
                        num4 -= 0.05f;
                    }
                    if (num4 <= 0f)
                    {
                        num4 += 24f;
                    }
                    value4.timeOfDay.GotoTime(num4);
                }
            }
            
            if (Input.GetKeyDown(KeyCode.Keypad9))
            {
                var RandomPlayer = Helper.GetRandomPlayer();
                var LocalPlayer = ComponentManager<Network_Player>.Value;
                LocalPlayer.transform.position = RandomPlayer.transform.position;
            }
            
        }
        public void OnGUI()
        {
            if (Event.current.type != EventType.Repaint) //GUI Funktion wird nur einmal aufgerufen pro render
                return;
            Hax.RenderMenu();            
            Hax.RenderESP();
        }
    }

   
}

