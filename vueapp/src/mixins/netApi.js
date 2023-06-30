import netApi from "@/api/network"

export default
    {
        methods:
        {
            modelGetData(/*controller, fnCallback*/)
            {
                return netApi.get('Users', null, undefined, (data) => {
                    this.setData(data)
                }, () => {
                    this.setData(null)

                    //ElMessage.warning("Erro ao processar pedido")
                })
            },

            modelPostData()
            {

            }
        }
    }