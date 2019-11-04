<template>
    <div class="col mb-4">
        <div class="card border-left-primary shadow h-100 py-2">
          <div class="card-body">
            <div class="row no-gutters align-items-center">
              <div class="col mr-2">
                <div
                  class="text font-weight-bold text-primary text-uppercase mb-1">{{source.Name}} - {{source.Version}} / {{source.Tag}}</div>
              </div>
            </div>
            <b-form>
                <b-form-group
                    id="input-group-1"
                    label="Name">
                    <b-form-input
                        type="text"
                        required
                        v-model="source.Name"
                        placeholder="Name"
                        ></b-form-input>
                </b-form-group>
                <b-form-group
                    id="input-group-1"
                    label="Version">
                    <b-form-input
                        type="text"
                        required
                        v-model="source.Version"
                        placeholder="Version"
                        ></b-form-input>
                </b-form-group>
                <b-form-group
                    id="input-group-1"
                    label="Tag">
                    <b-form-input
                        type="text"
                        required
                        v-model="source.Tag"
                        placeholder="Tag"
                        ></b-form-input>
                </b-form-group>
                <b-form-group
                    id="input-group-1"
                    label="Secret">
                    <b-form-input
                        type="text"
                        required
                        v-model="source.Secret"
                        placeholder="Secret"
                        ></b-form-input>
                </b-form-group>
                <b-form-group
                    id="input-group-1"
                    label="Id">
                    <b-form-input
                        type="text"
                        readonly
                        v-model="source.SourceId"
                        placeholder="SourceId"
                        ></b-form-input>
                </b-form-group>
                <b-button @click="saveSource()" variant="primary">Save</b-button>
                <b-form-textarea rows="4" class="form-control" readonly v-model="configString"></b-form-textarea>
            </b-form>
          </div>
        </div>
      </div>
</template>
<script>
import axios from 'axios';
export default {
    components:{
    },
    name:"SourceDetail",
    props:['source'],
    mounted(){
        console.log(this.source);
    },
    methods:{
        async saveSource(){
            try {
                if(this.source.SourceId !== undefined){
                    const response = await axios.put(`http://llogger.hopfenspace.org/api/Source/Update/` + this.source.SourceId,this.source);
                }else{
                    const response = await axios.post(`http://llogger.hopfenspace.org/api/Source/New`,this.source);
                }
                this.$emit('reload')
            } catch (e) {
                console.log(e);
            }
        }
    },
    computed:{
        configString(){
            return "\"LokiConfig\": {\r\n    \"Secret\": \"" + this.source.Secret + "\",\r\n    \"HostName\": \"https://llogger.hopfenspace.org/api/Logging/Log/" + this.source.SourceId + "\",\r\n    \"SendInterval\": 5\r\n  }"
        }
    }
}
</script>