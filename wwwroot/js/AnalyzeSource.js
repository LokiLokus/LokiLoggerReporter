new Vue({
    data:function(){
        return{
            data:[],
            selData:{},
            errors:{},
            source:"",
            allow:{
                debug:false,
                info:false,
                warn:true,
                err:true,
                crit:true
            }
        }
    },
    methods: {
        getData: function () {
            axios.get('/api/Logging/GetLogBySource/' + this.source + '/0-100')
                .then(x => {
                    this.data = x.data.filter(xa =>{
                        if(((xa.logLevel === 0 || xa.logLevel === 1) && this.allow.debug)) return true;
                        if(xa.logLevel === 2 && this.allow.info) return true;
                        if(xa.logLevel === 3 && this.allow.warn) return true;
                        if(xa.logLevel === 4 && this.allow.err) return true;
                        if(xa.logLevel === 5 && this.allow.crit) return true;
                        return false;
                    });
        })
        .catch(x => {
                if(x.response){
                this.errors = x.response.data;
            }else{
                    console.log(x)
                alert("Ein Fehler ist aufgetreten");
            }
        });
        },
        selectData: function (data) {
            this.selData = JSON.parse(JSON.stringify(data));
        },

    },
    mounted: function () {
    },
    computed: {
        isEmpty: function () {
            return isEmpty(this.selData)
        }
    },
    filters: {
        date: function (value) {
            if (!value) return '';
            var t = new Date(value);
            return t.toLocaleTimeString();
        },
        message:function (value) {
            if (!value || !value.message) return '';
            var result = value.message.replace(/ {[\s\S]*?}/g, value.data);
            return result
        }
    },
    el:"#analyzeSource"
});