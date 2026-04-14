import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { metasApi } from './api'
import type { CreateMetaInput, UpdateMetaInput } from './types'

const METAS_QUERY_KEY = ['metas'];
const META_PROGRESSO_QUERY_KEY = (id: string) => ['meta-progresso', id];
const ALERTAS_QUERY_KEY = ['alertas-orcamento'];

export const useListMetas = () => {
  return useQuery({
    queryKey: METAS_QUERY_KEY,
    queryFn: () => metasApi.listMetas(),
    staleTime: 5 * 60 * 1000, // 5 minutes
  });
};

export const useGetMeta = (id: string, options?: { enabled?: boolean }) => {
  return useQuery({
    queryKey: ['meta', id],
    queryFn: () => metasApi.getMeta(id),
    staleTime: 5 * 60 * 1000,
    enabled: options?.enabled !== false && !!id,
  });
};

export const useGetMetaProgresso = (id: string, options?: { enabled?: boolean }) => {
  return useQuery({
    queryKey: META_PROGRESSO_QUERY_KEY(id),
    queryFn: () => metasApi.getMetaProgresso(id),
    staleTime: 1 * 60 * 1000,
    enabled: options?.enabled !== false && !!id,
  });
};

export const useCreateMeta = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: CreateMetaInput) => metasApi.createMeta(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: METAS_QUERY_KEY });
      queryClient.invalidateQueries({ queryKey: ALERTAS_QUERY_KEY });
    },
  });
};

export const useUpdateMeta = (id: string) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: UpdateMetaInput) => metasApi.updateMeta(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: METAS_QUERY_KEY });
      queryClient.invalidateQueries({ queryKey: ['meta', id] });
      queryClient.invalidateQueries({ queryKey: META_PROGRESSO_QUERY_KEY(id) });
      queryClient.invalidateQueries({ queryKey: ALERTAS_QUERY_KEY });
    },
  });
};

export const useDeleteMeta = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) => metasApi.deleteMeta(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: METAS_QUERY_KEY });
      queryClient.invalidateQueries({ queryKey: ALERTAS_QUERY_KEY });
    },
  });
};

export const useListAlertas = () => {
  return useQuery({
    queryKey: ALERTAS_QUERY_KEY,
    queryFn: () => metasApi.listAlertas(),
    staleTime: 5 * 60 * 1000, // 5 minutes
  });
};
