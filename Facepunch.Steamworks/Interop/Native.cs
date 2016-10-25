﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Facepunch.Steamworks.Interop
{
    internal class NativeInterface : IDisposable
    {
        internal SteamNative.SteamApi api;
        internal SteamNative.SteamClient client;
        internal SteamNative.SteamUser user;
        internal SteamNative.SteamApps apps;
        internal SteamNative.SteamFriends friends;
        internal SteamNative.SteamMatchmakingServers servers;
        internal SteamNative.SteamInventory inventory;
        internal SteamNative.SteamNetworking networking;
        internal SteamNative.SteamUserStats userstats;
        internal SteamNative.SteamUtils utils;
        internal SteamNative.SteamScreenshots screenshots;
        internal SteamNative.SteamHTTP http;
        internal SteamNative.SteamUGC ugc;
        internal SteamNative.SteamGameServer gameServer;
        internal SteamNative.SteamGameServerStats gameServerStats;
        internal SteamNative.SteamRemoteStorage remoteStorage;

        internal bool InitClient()
        {
            api = new SteamNative.SteamApi( (IntPtr) 1 );

            api.SteamAPI_Init();

            var user = api.SteamAPI_GetHSteamUser();
            var pipe = api.SteamAPI_GetHSteamPipe();
            if ( pipe == 0 )
                return false;

            FillInterfaces( user, pipe );

            return true;
        }

        internal bool InitServer( uint IpAddress /*uint32*/, ushort usPort /*uint16*/, ushort GamePort /*uint16*/, ushort QueryPort /*uint16*/, int eServerMode /*int*/, string pchVersionString /*const char **/)
        {
            api = new SteamNative.SteamApi( (IntPtr)1 );

            api.SteamInternal_GameServer_Init( IpAddress, usPort, GamePort, QueryPort, eServerMode, pchVersionString );

            var user = api.SteamGameServer_GetHSteamUser();
            var pipe = api.SteamGameServer_GetHSteamPipe();
            if ( pipe == 0 )
                return false;

            FillInterfaces( pipe, user );

            if ( !gameServer.IsValid )
            {
                gameServer = null;
                throw new System.Exception( "Steam Server: Couldn't load SteamGameServer012" );
            }

            return true;
        }

        public void FillInterfaces( int hpipe, int huser )
        {
            var clientPtr = api.SteamInternal_CreateInterface( "SteamClient017" );
            if ( clientPtr == IntPtr.Zero )
            {
                throw new System.Exception( "Steam Server: Couldn't load SteamClient017" );
            }

            client = new SteamNative.SteamClient( clientPtr );

            user = client.GetISteamUser( huser, hpipe, "SteamUser019" );
            utils = client.GetISteamUtils( hpipe, "SteamUtils008" );
            networking = client.GetISteamNetworking( huser, hpipe, "SteamNetworking005" );
            gameServerStats = client.GetISteamGameServerStats( huser, hpipe, "SteamGameServerStats001" );
            http = client.GetISteamHTTP( huser, hpipe, "STEAMHTTP_INTERFACE_VERSION002" );
            inventory = client.GetISteamInventory( huser, hpipe, "STEAMINVENTORY_INTERFACE_V001" );
            ugc = client.GetISteamUGC( huser, hpipe, "STEAMUGC_INTERFACE_VERSION008" );
            apps = client.GetISteamApps( huser, hpipe, "STEAMAPPS_INTERFACE_VERSION008" );
            gameServer = client.GetISteamGameServer( huser, hpipe, "SteamGameServer012" );
            friends = client.GetISteamFriends( huser, hpipe, "SteamFriends015" );
            servers = client.GetISteamMatchmakingServers( huser, hpipe, "SteamMatchMakingServers002" );
            userstats = client.GetISteamUserStats( huser, hpipe, "STEAMUSERSTATS_INTERFACE_VERSION011" );
            screenshots = client.GetISteamScreenshots( huser, hpipe, "STEAMSCREENSHOTS_INTERFACE_VERSION002" );
            remoteStorage = client.GetISteamRemoteStorage( huser, hpipe, "STEAMREMOTESTORAGE_INTERFACE_VERSION013" );
        }

        public void Dispose()
        {
            if ( client != null )
            {
                client = null;
            }

            api.SteamAPI_Shutdown();
        }
    }
}