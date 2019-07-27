new Vue({
    data:function(){
        return{
            data:[],
            selData:{},
            errors:{},
            source:"",
            count:100,
            allow:{
                debug:true,
                info:true,
                warn:true,
                err:true,
                crit:true
            }
        }
    },
    methods: {
        getSource: function(){
            var url = window.location.pathname;
            var urls = url.split("/");
            var path;
            for (var i = 0; i < urls.length; i++) {
                if(urls[i] === "Source"){
                    if(urls.length > i){
                        this.source = urls[i+1];
                        break;
                    }
                }
            }
            
        },       
        
        getData: function () {
            axios.get('/api/Logging/GetLogBySource/' + this.source + '/0-' + this.count)
                .then(x => {
                    this.data = x.data.filter(xa =>{
                        if(((xa.LogLevel === 0 || xa.LogLevel === 1) && this.allow.debug)) return true;
                        if(xa.LogLevel === 2 && this.allow.info) return true;
                        if(xa.LogLevel === 3 && this.allow.warn) return true;
                        if(xa.LogLevel === 4 && this.allow.err) return true;
                        if(xa.LogLevel === 5 && this.allow.crit) return true;
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
        this.getSource();
        this.getData();
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
            if (!value || !value.Message) return '';
            var result = value.Message.replace(/ {[\s\S]*?}/g, value.Data);
            return result
        }
    },
    el:"#analyzeSource"
});