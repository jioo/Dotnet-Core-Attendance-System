import * as signalR from '@aspnet/signalr'
import JwtService from '@/services/jwt-service'

const connection = new signalR.HubConnectionBuilder()
    .withUrl(process.env.VUE_APP_BASE_URL + 'broadcast', {
        accessTokenFactory: () => JwtService.getToken()
    })
    .build();

export default connection