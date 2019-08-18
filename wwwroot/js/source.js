var app = new Vue({
    data:function(){
        return{
            data:[],
            loaded:false,
            selData:{}
        }
    },
    methods: {
        getData: function () {
            axios.get('/api/Source/All')
                .then(x => {
                    this.data = x.data;
                    this.loaded = true;
            }).catch(x => {
                if(x.response){
                this.errors = x.response.data;
            }else{
                console.log(x)
                alert("Ein Fehler ist aufgetreten");
            }
        });
        }
    },
    mounted: function () {
        this.getData()
    },
    computed: {
        isEmpty: function () {
            return isEmpty(this.selData)
        },
        
    },
    filters: {
    },
    el:"#adminvue"
});