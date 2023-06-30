import { isEmpty, isString, isObject } from 'lodash-es'

export default
{
    methods:
    {
        handleRoutePush(routeObj)
        {
            if(!isEmpty(routeObj) && (isString(routeObj) || isObject(routeObj)))
                this.$router.push(routeObj)
        },

        handleRouteReplace(routeObj)
        {
            if(!isEmpty(routeObj) && (isString(routeObj) || isObject(routeObj)))
                this.$router.replace(routeObj)
        },

        goBack(levels = 1, alternativeRoute = { name: 'home' })
        {
            // Verifica se há histórico de navegação disponível
            if (window.history.length > levels) {
                // Se houver histórico, volta uma página
                this.$router.go(-(levels));
            } else {
                // Se não houver histórico, redireciona para uma página específica
                this.$router.replace(alternativeRoute);
            }
        }
    }
}