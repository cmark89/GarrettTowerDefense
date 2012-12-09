using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace GarrettTowerDefense
{
    static class AudioManager
    {
        public static List<Song> MusicTracks { get; private set; }
        public static List<SoundEffect> SoundEffects { get; private set; }

        public static void Initialize()
        {
            MusicTracks = new List<Song>();
            SoundEffects = new List<SoundEffect>();
            //Load audio settings from file
        }

        public static void LoadContent(ContentManager Content)
        {
            AddSong(Content.Load<Song>("Audio/Music/AbandonAll"));
            AddSong(Content.Load<Song>("Audio/Music/AfflictionForever"));
            AddSong(Content.Load<Song>("Audio/Music/Bananalord"));
            AddSong(Content.Load<Song>("Audio/Music/Elegy"));
            AddSong(Content.Load<Song>("Audio/Music/Esoterica"));
            AddSong(Content.Load<Song>("Audio/Music/ProfaningTheRitual"));
            AddSong(Content.Load<Song>("Audio/Music/AbandonAllHQ"));

            AddSoundEffect(Content.Load<SoundEffect>("Audio/SoundEffects/Laugh"));
            AddSoundEffect(Content.Load<SoundEffect>("Audio/SoundEffects/LikeThat"));
            AddSoundEffect(Content.Load<SoundEffect>("Audio/SoundEffects/ArrowTower_Attack"));
            AddSoundEffect(Content.Load<SoundEffect>("Audio/SoundEffects/FireTower_Attack"));
            AddSoundEffect(Content.Load<SoundEffect>("Audio/SoundEffects/GlaiveTower_Attack"));
            AddSoundEffect(Content.Load<SoundEffect>("Audio/SoundEffects/GlaiveTower_Bounce"));
            AddSoundEffect(Content.Load<SoundEffect>("Audio/SoundEffects/GoldMine_Income"));
            AddSoundEffect(Content.Load<SoundEffect>("Audio/SoundEffects/IceTower_Attack"));
            AddSoundEffect(Content.Load<SoundEffect>("Audio/SoundEffects/TeslaTower_Attack"));
            AddSoundEffect(Content.Load<SoundEffect>("Audio/SoundEffects/Tower_Build"));
            AddSoundEffect(Content.Load<SoundEffect>("Audio/SoundEffects/Tower_Explode"));
            AddSoundEffect(Content.Load<SoundEffect>("Audio/SoundEffects/Tower_Upgrade"));
            AddSoundEffect(Content.Load<SoundEffect>("Audio/SoundEffects/ToxicTower_Attack"));

            AddSoundEffect(Content.Load<SoundEffect>("Audio/SoundEffects/Boss_Beam"));
            AddSoundEffect(Content.Load<SoundEffect>("Audio/SoundEffects/Boss_ShieldChange"));
            AddSoundEffect(Content.Load<SoundEffect>("Audio/SoundEffects/Boss_StunOrb"));
            AddSoundEffect(Content.Load<SoundEffect>("Audio/SoundEffects/Tower_Stunned"));
        }


        //Add the selected song to the list of songs
        public static void AddSong(Song song)
        {
            MusicTracks.Add(song);
        }

        public static void AddSoundEffect(SoundEffect se)
        {
            SoundEffects.Add(se);
        }

        public static Song GetSongByName(string name)
        {
            foreach (Song song in MusicTracks)
            {
                if (song.Name == name)
                    return song;
                else
                    continue;
            }

            return null;
        }

        public static void PlaySong(Song song)
        {
            try
            {

                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(song);
            }
            catch
            {
            }
        }

        public static void PlaySong(int index)
        {
            try
            {
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(MusicTracks[index]);
                
            }
            catch
            {
            }
        }


        public static void SetVolume(float vol)
        {
            MediaPlayer.Volume = vol;
        }


        public static void PlaySoundEffect(int index, float volume = .48f)
        {
            SoundEffects[index].Play(volume, 0f, 0f);
        }


        public static void StopMusic()
        {
            MediaPlayer.Stop();
        }
    }
}
