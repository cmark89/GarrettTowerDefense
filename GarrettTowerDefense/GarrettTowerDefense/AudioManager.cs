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

            AddSoundEffect(Content.Load<SoundEffect>("Audio/SoundEffects/Laugh"));
            AddSoundEffect(Content.Load<SoundEffect>("Audio/SoundEffects/LikeThat"));
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
                MediaPlayer.Play(MusicTracks[index]);
            }
            catch
            {
            }
        }

        public static void PlaySoundEffect(int index)
        {
            SoundEffects[index].Play();
        }

        public static void StopMusic()
        {
            MediaPlayer.Stop();
        }
    }
}
