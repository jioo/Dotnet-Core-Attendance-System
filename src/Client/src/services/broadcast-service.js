import * as signalR from '@aspnet/signalr'

const connection = new signalR.HubConnectionBuilder()
            .withUrl(process.env.VUE_APP_BASE_URL + 'broadcast')
            .build();

connection.start().catch(err => document.write(err));

export default connection