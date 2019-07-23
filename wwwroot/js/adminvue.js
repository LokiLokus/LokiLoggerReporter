var app = new Vue({
    data:function(){
        return{
            data:[]
        }
    },
    methods: {
        getData: function () {
            axios.get('/api/Source/All/')
                .then(x => {
                    this.data = x.data;
            }).catch(x => {
                if(x.response){
                this.errors = x.response.data;
            }else{
                console.log(x)
                alert("Ein Fehler ist aufgetreten");
            }
        });
        },
        getState: function (x) {
            if(x.Level[5].Count > 0){
                return 'bg-critical';
            }
            if(x.Level[4].Count > 0){
                return 'bg-danger';
            }
            if(x.Level[3].Count > 0){
                return 'bg-warning';
            }
            if(x.Level[2].Count > 0){
                return 'bg-info';
            }
            if(x.Level[1].Count > 0){
                return 'bg-success';
            }
            if(x.Level[0].Count > 0){
                return 'bg-success';
            }
            return 'bg-success';
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