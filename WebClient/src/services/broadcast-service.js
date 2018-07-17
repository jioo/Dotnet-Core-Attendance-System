import * as signalR from "@aspnet/signalr"
import { BASE_URL } from '@/config.js'

const connection = new signalR.HubConnectionBuilder()
            .withUrl(BASE_URL + "broadcast")
            .build();

connection.start().catch(err => document.write(err));

export default connection